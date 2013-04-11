// Define the Urdms.FlowWeb namespace
var Urdms = Urdms || {};
Urdms.FlowWeb = Urdms.FlowWeb || {};
Urdms.FlowWeb.Custom = Urdms.FlowWeb.Custom || {};
Urdms.FlowWeb.tinyMCESetup = function(ed) {
	var updateCharCount = function(ed) {
		var htmlContent = ed.getContent();
		var textContent = htmlContent.replace(/(<([^>]+)>)/ig, "");
		var nWords = textContent.length > 0 ? textContent.split(" ").length : 0;
		var nChars = textContent.length;
		var nHtmlChars = htmlContent.length;
		var text = nWords + " Word" + (nWords != 1 ? "s" : "") + ", "
			+ nChars + " Character" + (nChars != 1 ? "s" : "");
		var rMatch = ed.getElement().className.match(/maxlength\((\d+)\)/);
		if (rMatch && rMatch[1]) {
				text += ", " + ed.getContent().length + "/" + rMatch[1] + " HTML Characters";
		}
		tinymce.DOM.setHTML(tinymce.DOM.get(ed.id + '_path_row'), text); 	
	};
	ed.onKeyUp.add(function(ed, e) {  
		updateCharCount(ed);
	});
	ed.onInit.add(function(ed) {
		updateCharCount(ed);
		tinyMCE.dom.Event.add(ed.getWin(), "blur", function(e){
			var el = $(ed.getElement());
			if (el.val() != ed.getContent()) {
				ed.save();
				el.change()
			}
		});
	});
	// Focus the tiny mce when the textarea gets focus or when a label pointing to the textarea gets clicked
	// Reset the content in tinymce when reset buttons are clicked
	try {
		$("#"+ed.id).focus(function() {
			ed.focus();
		});
		$("label[for="+ed.id+"]").live("click", function() {
			$("#"+ed.id).focus();
		});
		$("form.flow").find("input[type=reset]").click(function() {
			ed.setContent(ed.startContent, {format: "raw"});
		});
	} catch(e) {}
}
if (typeof Urdms.FlowWeb.loadTinyMCE === "undefined") {
	Urdms.FlowWeb.loadTinyMCE = false;
}
Urdms.FlowWeb.tinyMCEInit = function(overwrite, extend) {
	var defaultOptions = {
		mode : "textareas",
		theme : "advanced",
		dialog_type : "modal",
		gecko_spellcheck : true,
		object_resizing : false,
		skin : "o2k7",
		skin_variant : "silver",
		fix_list_elements : true,
		fix_table_elements : true,
		invalid_elements : "script,style,link,head,title,meta,body",
		button_tile_map : true,
		remove_trailing_nbsp : true,
		verify_css_classes : true,
		theme_advanced_blockformats : "p,h1,h2,h3,h4,h5,h6,blockquote",
		theme_advanced_toolbar_location : "top",
		theme_advanced_toolbar_align : "left",
		plugins : "xhtmlxtras,inlinepopups,paste,safari,searchreplace,table,cu_iespell",
		theme_advanced_buttons1 : "cut,copy,paste,pasteword,removeformat,|,undo,redo,|,link,unlink,sub,sup,abbr,charmap,code",
		theme_advanced_buttons2 : "formatselect,cleanup,iespell,|,justifyleft,justifycenter,justifyright,justifyfull,|,bold,italic,underline,|,selectall,search,replace",
		theme_advanced_buttons3 : "numlist,bullist,outdent,indent,|,tablecontrols",
		theme_advanced_statusbar_location : "bottom",
		theme_advanced_path : false,
		theme_advanced_resizing : true,
		theme_advanced_resize_horizontal : false,
		setup : Urdms.FlowWeb.tinyMCESetup,
		content_css : "Content/Css/tinymce.css"
	};
	var opts = defaultOptions;
	for (var prop in overwrite) {
		opts[prop] = overwrite[prop];
	}
	for (var prop in extend) {
		if (opts.hasOwnProperty(prop)) {
			opts[prop] += "," + extend[prop];
		} else {
			opts[prop] = extend[prop];	
		}
	}
	tinyMCE.init(opts);
};
if (typeof Urdms.FlowWeb.setupDirtyTracking === "undefined") {
	Urdms.FlowWeb.setupDirtyTracking = function() {
		// Flag to keep track of form status
		var formDirty = false;
		var f = $("form.flow");
		var resets = f.find("input[type=reset]");
		var setClean = function() {
			formDirty = false;
			resets.attr("disabled", "disabled");
		};
		var setDirty = function() {
			formDirty = true;
			resets.attr("disabled", "");
		};
		
		// The form starts off clean
		setClean();
		
		// Set the flag to true if a field is edited
		f.find("select,input,textarea").not("input[type=submit], input[type=reset]").change(function() {
			setDirty();
		});
		// Display a message to the user if the form is dirty when they try and leave it
		window.onbeforeunload = function() {
			if (formDirty) {
				return "You have unsaved information on this page. Are you sure you want to exit the page without saving?";
			}
		}
		
		// Ensure any submit buttons reset the dirty flag if the default behaviour
		if (Urdms.FlowWeb.allSubmitButtonsAllowSubmit) {
			f.find('input[type=submit]').click(function() {
				setClean();
				return true;
			});
		}
		
		// Ensure any reset buttons reset the dirty flag
		resets.click(function() {
			setClean();
			$(this).blur();
			return true;
		});
		
		return {
			isDirty: function() {
				return formDirty;
			},
			setClean: setClean
		};
	}
}
if (typeof Urdms.FlowWeb.allSubmitButtonsAllowSubmit === "undefined") {
	Urdms.FlowWeb.allSubmitButtonsAllowSubmit = true;
}
Urdms.FlowWeb.focusFirstErrorElement = function() {
	var firstErrorElement = $("#content form.flow select.error, #content form.flow input.error, #content form.flow textarea.error").eq(0);
	// If the first element is hidden then scroll to it
	if (firstErrorElement.css("display") === "none") {
		$(window).scrollTop(firstErrorElement.show().position().top);
		firstErrorElement.hide();
	}
	firstErrorElement.focus();
}

/**
 * Determine the protocol needed for the javascript requests based on the window location.
 */
var protocol_prefix = window.location.href.match(/^https:/) ? 'https://' : 'http://'
var environment = ".";
var version = "2.0.0";

/**
 * Entry point.
 *
 * NOTE: the main function is simply defined at the end of the file.
 */
function main() {
	include_libraries(
		[jquery_loader],
			/**
			 * The following is nested due to the dependency on jQuery
			**/
		function() {
			var match = []
			$("head script").each(function() {
				if ($(this).attr("src")) {
					if (match = $(this).attr("src").match( /(?:https?:)?\/\/your(\.(?:test\.|dev\.)?)domain\.edu\.au\/flow\/forms-\d\.\d\.\d\/javascript\/forms\.js/ )) {
						environment = match[1]; // You can determine environment.
					}
				}
			});
			include_libraries(
				[
					jquery_datepicker_loader,
					jquery_validate_loader,
					jquery_qtip_loader,
					tiny_mce_loader
				],
				function() {
					jquery_patch_chrome_date()
					$(function() {

						/***
									 * All of the following functionality is placed in global functions so that
									 * users can overwrite the routines if desired.
									 ***/
						extend_validation();
						parametized_classes();
						tips();
						hints();
						validation();
						datePicker();
					});
				}
			);
		}
	);
}

function jquery_loader() {
	return (!window.jQuery) && ("Scripts/jquery-1.4.4.min.js");
}

function sub_jquery(component, library) {
	try {
		return (!traverse(component, window.jQuery)) && library;

	} catch(e) {return false}
}

function jquery_datepicker_loader() { return sub_jquery('ui', "/Scripts/datepicker.packed.js") }
function jquery_validate_loader() { return sub_jquery('validator', "/Scripts/jquery.validate-1.6.0.pack.js") }
function jquery_qtip_loader() { return sub_jquery('fn.qtip', "/Scripts/jquery.qtip-1.0.0-rc3.min.js") }
function tiny_mce_loader() { return (! !Urdms.FlowWeb.loadTinyMCE) && (! window.tinymce) && ("/Scripts/tiny_mce-3.3.9.2.js") }

/*
 * Show / Hide dependency function
 */
function depends(id, dependsOn, dependsOnValue, negate, dependsOnIsId, hideFieldOnly) {

	if (typeof dependsOnValue === "undefined") {
		dependsOnValue = null;
	}
	if (typeof negate === "undefined") {
		negate = false;
	}
	if (typeof dependsOnIsId === "undefined") {
		dependsOnIsId = false;
	}
	if (typeof hideFieldOnly === "undefined") {
		hideFieldOnly = false;
	}
	
	dependsOn = (!dependsOnIsId ? "input[name=" + dependsOn + "]" : "#" + dependsOn);

	var status = function() {
		var stat = false;
		if ($(dependsOn).is(":checkbox, :radio")) {
			stat = $(dependsOn + (dependsOnValue != null ? "[value=" + dependsOnValue + "]" : "") + ":checked").size() != 0
		} else {
			stat = $(dependsOn).val() == dependsOnValue;
		}
		return stat;
	};
	var hide = function() {
		var e = $("#" + id);
		if (e.not(":checkbox,:radio").is("input, textarea")){
			e.val("").keyup();	
		}
		else if(e.is("select")){
			e.val("").click();		
		}
		else{
			$("input[name=" + id + "]").attr("checked", "").click();
		}
		
		if (!hideFieldOnly) {
			e.closest("dl").hide();
		} else {
			e.closest("dd").add(e.closest("dd").prev("dt")).hide();
		}
		
	};
	var show = function() {
		var e = $("#" + id);
		if (!hideFieldOnly) {
			e.closest("dl").show();
		} else {
			e.closest("dd").add(e.closest("dd").prev("dt")).show();
		}
	};
	if ((!negate && !status()) || (negate && status())) {
		hide();
	}
	if ($(dependsOn).is(":radio, :checkbox, select")) {
		$(dependsOn).change(function() {
			if ((!negate && !status()) || (negate && status())) {
				hide();
			} else {
				show();
			}
		});
	} else {
		$(dependsOn).keyup(function() {
			if ((!negate && !status()) || (negate && status())) {
				hide();
			} else {
				show();
			}
		});		
	}
}


/*
 * Add support for further validation classes and methods.
 */
function extend_validation() {
	/*
	 * Parametized "match" method for regex and strings.
	 */
	jQuery.validator.addMethod(
		"match",
		function(value, element, arg) {
			// Allow empty values for 'required' orthogonallity
			if (value.match( /^\s*$/ )) {
				return true;
			}
			return value.match(arg);
		},
		jQuery.format("Please ensure the value is in the correct format.")
	);

	/*
	 * ip-address method.
	 */
	jQuery.validator.addMethod(
		"ip_address",
		function(value, element) {

			// Allow empty values for 'required' orthogonallity
			if (value.match( /^\s*$/ )) {
				return true;
			}

			// Match against a liberal ip grouping
			var match = value.match( /^([0-9]{1,3})\.([0-9]{1,3})\.([0-9]{1,3})\.([0-9]{1,3})$/ )
			if (!match) {
				return false;
			}

			// Check that each group is < 255
			for (var i = 1; i <= 4; i++) {
				if (Number(match[i]) > 255) {
					return false;
				}
			}
			return true;
		},
		jQuery.format("Please ensure the value is a valid IP address.")
	);
	jQuery.validator.addClassRules("ip-address", {
		"ip_address": true
	});
	
	/*
	 * integer method.
	 */
	jQuery.validator.addMethod(
		"integer",
		function(value, element) {

			// Allow empty values for 'required' orthogonallity
			if (value.match( /^\s*$/ )) {
				return true;
			}

			// Match an integer (allowing leading + or -)
			if (value.match( /^[+-]?\d+$/ )) {
				return true;
			}

			return false;
		},
		jQuery.format("Please ensure the value is a valid integer number.")
	);
	jQuery.validator.addClassRules("integer", {
		"integer": true
	});
}

/**
 * User overwritable functions
**/
function datePicker() {
	// Datepicker
	$('#content form.flow input[type=text].date').each(function() {
		var opts = { formElements: { } };
		if ($(this).attr("id").length > 0)
			opts.formElements[$(this).attr("id")] = 'd-sl-m-sl-Y';

		datePickerController.createDatePicker(opts);
	});
}

function validation() {
	$('.mandatory').each(function() {
		var sub = $(this);
		sub.find('>select, >textarea, >input[type!=radio][type!=checkbox]').addClass('required');
		sub.find('ul>li>input[type=radio]:first').addClass('required');
		sub.find('ul>li>input[type=checkbox]:first').addClass('required');
		sub = sub.find('>table');
		if (sub.size() > 0) {
			sub.find('select, textarea, input[type!=radio][type!=checkbox]').addClass('required');
			sub.find('input[type=radio]:first').addClass('required');
			sub.find('input[type=checkbox]:first').addClass('required');
		}
	});

	$('form.flow select, form.flow textarea, form.flow input').live("valid", function() {
		var eElement = errorElement($(this));
		if (eElement.data("qtip")) {
			eElement.qtip("destroy").data("errorMessage", "");
		}
	});

	function errorElement(element) {
		var eElement = element.closest('dd').prevAll('dt:first');
		if (eElement.find("label").size() > 0) {
			eElement = eElement.find("label");
		}
		if (element.parents("label").size() > 0) {
			eElement = element;
		}
		if (element.parents("#content.no_menu").size() > 0 && 
				element.parents("form.flow.condensed").size() > 0) {
			eElement = element;
			if (element.is("input.date") && element.next().is(".date-picker-control")) {
				eElement = element.next();
			}
		}
		return eElement;
	}

	function errorTipShower(error, element) {
		var errorMessage = error.text();
		var showTip = true;
		var showCondition = { ready: true };
		var hideCondition = { when: { 'event': 'never' } };
		var target = 'leftMiddle';
		var tooltip = 'rightMiddle';
		
		if ($(element).attr("id"))
			errorMessage = "<label for=\"" + $(element).attr("id") + "\">" + errorMessage + "</label>";

		var qtipElement = errorElement($(element));
		// If the element has a label around it then put the error tip over that
		// 	since it's probably in a grid control, also the hint
		//	will hide the other fields unless they show and hide on focus / blur
		if (element.parents("label").size() > 0) {
			showCondition = { when: { 'event': 'focus' } };
			hideCondition = { when: { 'event': 'blur' } };
		}
		
		if ($(element).parents("#content.no_menu").size() > 0) {
			target = 'rightMiddle';
			tooltip = 'leftMiddle';
		}

		if (qtipElement.data("qtip")) {
			if (qtipElement.data("errorMessage") != error.text()) {
				qtipElement.qtip("destroy");
			} else {
				showTip = false;
			}
		}
		
		qtipElement.data("errorMessage", error.text());
		
		if (showTip) {
			qtipElement.qtip({
				content: errorMessage,
				style: {
					name: 'red',
					width: 176,
					tip: tooltip,
					background: '#FFE6E6',
					border: { color: 'red', width: 2 }
				},
				show: showCondition,
				hide: hideCondition,
				position: {
					corner: {
						target: target,
						tooltip: tooltip
					},
					adjust: {
						x: 5 * (target == 'leftMiddle' ? -1 : 1)
					}
				}
			});
		}
	}

	var validationArgs = { };
	
	// Allow users to define custom validation arguments
	if (typeof(Urdms.FlowWeb.Custom.validationFunction) !== "undefined")
		var validationArgs = Urdms.FlowWeb.Custom.validationFunction();

	// Add the errorPlacement handler to the arguments if the user dind't overwrite it
	if (typeof(validationArgs.errorPlacement) === "undefined")
		validationArgs.errorPlacement = errorTipShower;

	// Add the validation callback to the form
	var val = $('#content form.flow').validate(validationArgs);

	// Call the form start method if defined
	if (typeof(Urdms.FlowWeb.formStart) !== "undefined") {
		Urdms.FlowWeb.formStart();
	} else if (Urdms.FlowWeb.loadTinyMCE) {
		Urdms.FlowWeb.tinyMCEInit();
	}
	
	if (Urdms.FlowWeb.setupDirtyTracking)
		Urdms.FlowWeb.Dirty = Urdms.FlowWeb.setupDirtyTracking();

}

/*
 * This function loops over all classes that appear to be parametized and adds a corresponding rule to jquery validate.
 */
function parametized_classes() {
	var flow_form = $("#content form.flow");
	var elements = flow_form.find("input, select, textarea");
	var classes = elements.map(function(index, domElement) {
		return $(domElement).attr('class').split( /\s+/ );
	});

	var parametized = $.grep(classes, function(item, index) {
		// Regex matches function-call style classes
		return item.match( /^[^()\s]+\(\S*\)$/ );
	});

	var unique = $.unique(parametized);

	$.each(unique, function() {
		var pair = this.match( /^([^()\s]+)\((\S*)\)$/ );
		var name = pair[1];
		var argument = pair[2] || true;
		var rule = { };
		// The argument needs to be evaluated, as it could be a function
		rule[name] = eval(argument);
		jQuery.validator.addClassRules(this, rule);
	});
}

function tips() {
	
	// Column table header tips: Propagate to td's in that column
	$('#content form.flow table tr:first-child th[scope=col] .tip:not(.plain)').each(function() {
		var tip = $(this);
		// Find column number of this th
		var columnNo = tip.parent().parent().children().index(this.parentNode);
		// Get all the td's in that column through the table without tips already in them
		tip.closest("table").find("tr:not(:first-child)").find(">*:eq(" + columnNo + ")").filter(function() { return $(this).children(".tip").size() == 0 }).append(tip.clone().removeClass("retain"));
	});
	
	// Row table header tips: Propagate to td's in that row
	$('#content form.flow th:first-child[scope=row] .tip:not(.plain)').each(function() {
		var tip = $(this);
		tip.closest("tr").find("td").filter(function() { return $(this).children(".tip").size() == 0 }).append(tip.clone().removeClass("retain"));
	});
	
	// Tips
	$('#content form.flow dd').each(function() {
		var section = $(this);
		var tips = section.find('.tip').not('.plain');

		tips.each(function() {
			var tip = $(this);
			// TODO: Make this nest tooltips
			var element = tip.prevAll('*:not(.tip)').eq(0);

			var cornerTarget = 'rightMiddle';
			var cornerTooltip = 'leftTop';

			if (element.parents("table").size() > 0) {
				cornerTarget = 'bottomMiddle';
				cornerTooltip = 'topMiddle';
			}

			element.qtip({
				content: tip.html(),
				style: {
					name: 'cream',
					tip: 'leftMiddle',
					color: 'black',
					background: '#FFFFE6',
					border: { color: '#E5D6AB', width: 2 }
				},
				position: {
					corner: {
						target: cornerTarget,
						tooltip: cornerTooltip
					},
					adjust: {
						x: 5,
						screen: true
					}
				}
			}).focus(
				function() { $(this).mouseover(); }
			).blur(
				function() { $(this).mouseout(); }
			);

			if (!tip.hasClass("retain"))
				tip.hide();
		});
	}); // .sub_section.each
}

function hints() {
	// Hints
	$('#content form.flow dt').each(function() {
		var sectionTitle = $(this);
		var sectionBody = sectionTitle.next('dd');
		var hints = sectionBody.find('> .hint').not('.plain');

		if (hints.size() > 0) {

			hints.each(function() {
				var hint = $(this);

				// Use classes to modify the width of the hint
				var width = 300;
				if (hint.hasClass("wide")) {
					width = 600;
				}

				sectionTitle.qtip({
					content: hint.html(),
					style: {
						name: 'cream',
						tip: 'bottomLeft',
						color: 'black',
						background: '#FFFFCC',
						width: width,
						border: { color: '#965F00', width: 2 }
					},
					position: {
						corner: {
							target: "topLeft",
							tooltip: "bottomLeft"
						},
						adjust: {
							x: 20,
							screen: true
						}
					}
				}); // qtip

				hint.hide();
			}); // hints.each

			sectionBody.data("focussedElement", false).hover(
				function() { sectionTitle.mouseover(); },
				function() {
					if (!sectionBody.data("focussedElement") || sectionBody.data("focussedElement") == "false") {
						sectionTitle.mouseout();
					}
				}
			).find("input, select, textarea").focus(function() {
				sectionBody.data("focussedElement", true);
				sectionTitle.mouseover();
			}).blur(function() {
				sectionBody.data("focussedElement", false);
				sectionTitle.mouseout();
			});
		}
	});
}

/* Overriding the date validation function on Google Chrome,
 * as it fails to parse dd/mm/yyyy dates correctly.
 * For example: (new Date('13/02/2001')).toString()
 * returns "Invalid Date".
 * 
 * All browsers seem to parse dates in american format,
 * So we override for all browsers.
 *
 * The default is overridden as we always want dates in
 * Australian/International format "dd/mm/yyyy".
 */
function jquery_patch_chrome_date() {
	
	/* Method overridden to prevent chrome bug in date parsing.
	 * Quite verbose, but should be complete.
	 * TODO: Remove this once chrome can parse dates propperly again.
	 */
	$.validator.addMethod('date', function (value,element) {

		/* As per jQuery convention: Return a posetive result if the element is optional. */
		if(this.optional(element)) {
			return true;
		}

		/* Leap-year detection funciton.
		 * Based on http://en.wikipedia.org/wiki/Leap_year#Algorithm
		 */
		function leap(year) {
			if(year % 400 == 0)
				return true;
			if(year % 100 == 0)
				return false;
			if(year % 4 == 0)
				return true;
			return false;
		}

		/* Regex accepts any numeric triple seperated by slashes or hyphens.
		 * Any amount of whitespace outside of numbers is also allowed.
		 * The regex is anchored.
		 */
		var regex = /^\s*(\d+)\s*[/-]\s*(\d+)\s*[/-]\s*(\d+)\s*$/;

		var match = value.match(regex);

		if(! match) {
			return false;
		}

		var d = Number(match[1]);
		var m = Number(match[2]);
		var y = Number(match[3]);

		/* Perform some sanity (upper/lower bounds) checks on the day and month. */

		if((m < 1 || m > 12)) {
			return false;
		}

		if((d < 1 || d > 31)) {
			return false;
		}

		/* 30 days has November, April, June and December,
		 * all the rest have 31 except Feburary,
		 * which has 28 days clear, and 29 every leap-year.
		 */
		switch(m) {
			case 2: // Febuary

				if(leap(y)) {
					if(d > 29) {
						return false;
					}
				} else {
					if(d > 28) {
						return false;
					}
				}
				break;

			case 4: // April
				if(d > 30) { return false; }
				break;

			case 6: // June
				if(d > 30) { return false; }
				break;

			case 9: // September
				if(d > 30) { return false; }
				break;

			case 11: // November
				if(d > 30) { return false; }
				break;
		}
		
		return true;
	});
}

/**
 * Utility functions:
 *
 * These functions are used for library loading, etc.
**/

/**
 * Function: Traverse.
 *
 * Arguments:
 * - A path delimited by '.' or an Array of paths
 * - An object
 *
 * Returns the end of the path in the object.
 *
 * Throws an exception if there is no elelement at the path.
**/
function traverse(path, obj) {
	// Note this doesn't catch actuall String instances
	if(typeof path == "string") {
		path = path.split('.');
	}

	var current = obj;

	for(var i = 0; i < path.length; i++) {
		current = current[path[i]];
	}

	return current;
}

/**
 * Function: All.
 *
 * Arguments:
 * - An object.
 *
 * Takes an object and ensures that values for all fields are not null.
**/
function all(hash) {
	for(var key in hash) {
		if(! hash[key]) {
			return false;
		}
	}
	return true;
}

/**
 * Function: Include_libraries.
 *
 * Arguments:
 * - Set of libraries:
 *   * These are strings to load as urls, or functions that return a url string, or null to skip.
 *   * The reason to support the function type is so that tests can be performed to check if the
 *     library is already loaded.
 *
 * - Callback function:
 *   * Once the libraries are all loaded, the callback function is run.
 *
 * Returns:
 * - True if all libraries are loaded sucessfully
 * - False if something fails during loading
 *
 * This function takes a set of library paths,
 * it includes all libraries into the head,
 * then, when all libraries are loaded,
 * it executes the callback.
 *
 * TODO: Create a way to have callbacks per constraint:
 *       - i.e.
 *         * Total failure
 *         * Partial failures
 *
 * This function does not assume that any frameworks are loaded.
**/
function include_libraries(libraries, callback) {

	// Stores the state of which libraries are loaded.
	var loaded = new Object();

	/**
	 * initialize loaded to fully false
	 * only set keys for:
	 * - Strings
	 * - Strings returned from funcitons
	**/
	for(var i = 0; i < libraries.length; i++) {
		var intermediate = libraries[i];

		if(typeof intermediate == 'function') {
			var result = intermediate();
			if(result) {
				loaded[result] = false;
			}

		// If it isnt a function, then just assume it is a string for now.
		} else {
			loaded[intermediate] = false;
		}
	}

	// If for some reason, all libraries are already loaded,
	// then run the callback
	var empty = true;
	for(var hi in loaded) {
		empty = false;
	}

	if(empty) {
		callback();
		return true;
	}

	// load all libraries and call the callback when done
	for(var library in loaded) {

		// Done for scoping callback reasons
		;
		(function(lib_arg) {

			includeJs(
				lib_arg,
				function() {
					loaded[lib_arg] = true;

					if (all(loaded)) {
						// This will only be triggered once as the library strings are keys in a hash,
						// and thus unique
						callback();
					}
				}
			);

		})(library);
	}
}

/**
 * Author: Steve Bramley.
 *
 * Function appends a javascript file into the header of the page’s DOM
 * Instead of an arbitrary and lengthy timeout to ensure the script’s functions will be 
 * ready, we can use the completed upload event to trigger
**/
function includeJs(filename,callback){
	//get the page’s head object
	var head = document.getElementsByTagName('head')[0];

	//create an build the new scripting object to be injected
	var script = document.createElement('script');
	script.type = 'text/javascript';
	script.src = filename;

	//if we’re dealing with i.e.
	if (script.readyState){

		//check the status of the script looking for a completed load
		script.onreadystatechange= function () { 

			//check to see if the state has changed to indicate a successful load
			if (script.readyState == "loaded" || script.readyState == "complete"){ 

				//some scripts don’t need a callback as they’re self executing so avoid a callback if it’s not already defined
				if (callback != null){

					//the callback is the collection of operations waiting for the script to be uploaded.
					callback();
				}
			}
		}
	} else {

		//Mozilla based browsers use the onload event for upload completion
		script.onload = function () {

			if (callback){
				callback();
			}
		}
	}

	//attach/upload the js file to the document head
	head.appendChild(script);
}

main()