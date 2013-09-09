param($installPath, $toolsPath, $package, $project)
$projectPath = [System.IO.Path]::GetDirectoryName($project.FullName)
$projectNamespace = $project.Properties.RootNameSpace

Function UpgradeMigrations {
	param($migrationsPath, $toolsPath, $namespace)
	
	$hadOldMigrations = $false

	if (Test-Path $migrationsPath) {

		$migrationFiles = Get-ChildItem -recurse $migrationsPath *.cs

		if ($migrationFiles.Count -gt 0) {
			foreach($file in $migrationFiles) {
				$thisFileHadMigration = ReplaceMigratorDotNet $file
				$hadOldMigrations = $hadOldMigrations -or $thisFileHadMigration
			}
		}

		if ($hadOldMigrations) {
			$content = Get-Content ([System.IO.Path]::Combine($toolsPath, "20100101000000_UpdateToFluentMigrator.cstemp"))
			$content = $content -replace "%namespace%", $namespace
			[IO.File]::WriteAllLines([System.IO.Path]::Combine($migrationsPath, "20100101000000_UpdateToFluentMigrator.cs"), ($content -join "`r`n"))
		}
	}

	return $hadOldMigrations;
}

Function ReplaceMigratorDotNet {
    param($file)
    
	$content = Get-Content $file.FullName

	# Identify a MigratorDotNet Migration
	if ($content -match "using Migrator\.Framework;" -and $content -match "\[Migration\(") {
		Write-Host "Replacing migration " $file.Name
		$content = $content -replace "using Migrator.Framework;", "using FluentMigrator.Legacy.MigratorDotNet;`r`nusing FluentMigrator;"
		$content = $content -replace ":\s*Migration\b", ": MigratorDotNetMigration"
		[IO.File]::WriteAllLines($file.FullName, ($content -join "`r`n"))
		return $true;
	}

	return $false;
}

Function HasMigratorDotNet {
    param($projectPath)
    
    $packagesPath = [System.IO.Path]::Combine($projectPath, "packages.config")

    # If there is a packages.config file
    if (Test-Path $packagesPath) {
        $mdn = ([xml](Get-Content $packagesPath)).SelectNodes("/packages/package[@id='MigratorDotNet']")
        if ($mdn.Count -gt 0) {
            return $true
        }
    }
    return $false
}

Function RemoveMigratorDotNet {
	param($projectName, $projectPath, $projectNamespace, $toolsPath)

	# Is MigratorDotNet installed?
	$hasMigratorDotNet = HasMigratorDotNet $projectPath
    if (!$hasMigratorDotNet) {
        return
    }

	# Uninstall MigratorDotNet
	Write-Host "Project" $projectName "has MigratorDotNet installed; uninstalling and updating namespaces."
    if (Get-Command "Uninstall-Package" -errorAction SilentlyContinue) {
        Uninstall-Package MigratorDotNet
    } else {
        Write-Host
        Write-Warning "[!!!] Unable to find NuGet commands; please manually uninstall MigratorDotNet!"
        Write-Host
    }

	# Find all migration files and replace migratordotnet usage
	$hadOldMigrations = UpgradeMigrations ([System.IO.Path]::Combine($projectPath, "DatabaseSchema")) $toolsPath "$projectNamespace.DatabaseSchema"
	if (-not $hadOldMigrations) {
		$hadOldMigrations = UpgradeMigrations ([System.IO.Path]::Combine($projectPath, "Database", "Migrations")) $toolsPath "$projectNamespace.Database.Migrations"
	}

	# If there were old migrations then install the legacy NuGet package
	if ($hadOldMigrations) {
		Write-Host "Project" $projectName "had legacy MigratorDotNet migrations; installing FluentMigrator.Legacy.MigratorDotNet."
		if (Get-Command "Install-Package" -errorAction SilentlyContinue) {
			Install-Package Curtin.FluentMigrator.Legacy.MigratorDotNet
		} else {
			Write-Host
			Write-Warning "[!!!] Unable to find NuGet commands; please manually install FluentMigrator.Legacy.MigratorDotNet!"
			Write-Host
		}
	} else {
		Write-Host "No MigratorDotNet migrations to replace."
	}
}

RemoveMigratorDotNet $project.Name $projectPath $projectNamespace $toolsPath
