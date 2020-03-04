$(function() {

	/* Initializing the scrollbar */
	if($('.scrollbar-inner').size()>0){
		$('.scrollbar-inner').scrollbar({
			"disableBodyScroll":true
		});
	}

	if($(window).width() < 1199 ){
		if($('.scrollbar-variant').size()>0){
			$('.scrollbar-variant').scrollbar();
		}
	}

	if($('form.fn_fast_button').size()>0){
		$(document).on('change', '.fn_action_block:not(.fn_fast_action_block) select', function(e, trigger) {
			if (!trigger) {
				var name = $(this).attr('name'),
				selected = $(this).children(':selected').val();
				$('.fn_fast_save select[name="' + name + '"]').val(selected).trigger('change', {trigger: true});
			}
		});
		
		$(document).on('change', '.fn_fast_save select', function(e, trigger) {
			if (!trigger) {
				var name = $(this).attr('name'),
				selected = $(this).children(':selected').val();
				$('form.fn_fast_button select[name="' + name + '"]').val(selected).trigger('change', {trigger: true});
			}
		});
		
		
		if ($('.fn_action_block').size()>0) {
			var action_block = $('.okay_list_option').clone(true);
			$('.fn_fast_action_block .action').html(action_block);
			if ($('.fn_additional_params').size()) {
				var additional_params = $('.fn_additional_params').clone(true);
				$('.fn_fast_action_block .additional_params').html(additional_params);
			}
		}
		
		$('input,textarea,select, .dropdown-toggle, .fn_sort_item, .fn_category_item').bind('keyup change dragover click',function(){
			$('.fn_fast_save').show();
		});
		$('.fn_fast_save .fast_save_button').on('click', function () {
			$('body').find("form.fn_fast_button").trigger('submit');
		});
	}

	/* Check */
	if($('.fn_check_all').size() > 0) {
		$(document).on('change', '.fn_check_all', function() {
			if($(this).is(":checked")) {
				console.log($(this).closest("form").find('.hidden_check'))
				$(this).closest("form").find('.hidden_check').each(function() {
					if(!$(this).is(":checked")){
						$(this).trigger("click");
					}
				});
			} else {
				$(this).closest("form").find('.hidden_check').each(function() {
					if($(this).is(":checked")) {
						$(this).trigger("click");
					}
				})
			}
		});
	}
	/* Check end*/

	// Small script for tooltip
	$(function(){
		$(".fn_tooltips").tooltip();
	});

	/* Catalog items toggle */
	if($('.fn_item_switch').size()>0){
		$('.fn_item_switch').on('click',function(e){
			var parent = $(this).closest("ul"),
			li = $(this).closest(".fn_item_sub_switch"),
			sub = li.find(".fn_submenu_toggle");

			if(li.hasClass("open active")){

				sub.slideUp(200, function () {
					li.removeClass("open active")
				})

			} else {
				parent.find("li.open").children(".fn_submenu_toggle").slideUp(200),
				parent.find("li.open").removeClass("open active"),
				li.children(".arrow").addClass("open active"),

				sub.slideDown(200, function () {
					li.addClass("open active")
				})
			}
		});
	}

	/* Left menu toggle */
	if($('.fn_switch_menu').size()>0){
		$(document).on("click", ".fn_switch_menu", function () {
			$("body").toggleClass("menu-pin");
		});
		$(document).on("click", ".fn_mobile_menu", function () {
			$("body").toggleClass("menu-pin");
			$(".fn_quickview").removeClass("open");
		});
	}

	/* Right menu toggle */
	if($('.fn_switch_quickview').size()>0){
		$(document).on("click", ".fn_mobile_menu_right", function () {
			$(this).next().toggleClass("open");
			$("body").removeClass("menu-pin");
		});
		$(document).on("click", ".fn_switch_quickview", function () {
			$(this).closest(".fn_quickview").toggleClass("open");
		});
	}

	/* Script for products */
	$(document).on('click', '.fn_variants_toggle', function() {
		$(this).find('.fn_icon_arrow').toggleClass('rotate_180');
		$(this).parent('.fn_row').find('.products_variants_block').slideToggle();
	});

	$(document).on('change', '.fn_action_block select.products_action', function() {
		var elem = $(this).find('option:selected').val();
		$('.fn_hide_block').addClass('hidden');
		if($('.fn_' + elem).size() > 0){
			$('.fn_' + elem).removeClass('hidden');
		}
	});

	$(document).on('click', '.fn_show_icon_menu', function() {
		$(this).toggleClass('show');
	});
	/* Script for products end*/

	/* Script for fn_action_block button */

	$(document).on('change', '.fn_action_block select.brands_action', function() {
		var elem = $(this).find('option:selected').val();
		$('.fn_hide_block').addClass('hidden');
		if($('.fn_' + elem).size() > 0) {
			$('.fn_' + elem).removeClass('hidden');
		}
	});

	$(document).on('change', '.fn_action_block select.categories_action', function() {
		var elem = $(this).find('option:selected').val();
		$('.fn_hide_block').addClass('hidden');
		if($('.fn_' + elem).size() > 0) {
			$('.fn_' + elem).removeClass('hidden');
		}
	});

	/* Script for fn_action_block button end */


	/* Script for brands */

	$(document).on('change', '.fn_action_block select.brands_action', function () {
		var elem = $(this).find('option:selected').val();
		$('.fn_hide_block').addClass('hidden');
		if ($('.fn_' + elem).size() > 0) {
			$('.fn_' + elem).removeClass('hidden');
		}
	});

	/* Script for brands end*/

	/* Script for categories */

	$(document).on('change', '.fn_action_block select.categories_action', function () {
		var elem = $(this).find('option:selected').val();
		$('.fn_hide_block').addClass('hidden');
		if ($('.fn_' + elem).size() > 0) {
			$('.fn_' + elem).removeClass('hidden');
		}
	});

	/* Script for categories end*/
	

	/* Product duplication */
	$(document).on('click', ".fn_copy", function() {
		$('.fn_form_list input[type="checkbox"][name*="check"]').attr('checked', false);
		$(this).closest(".fn_form_list").find('select[name="action"] option[value=duplicate]').attr('selected', true);
		$(this).closest(".fn_row").find('input[type=checkbox][name*="check"]').attr('checked', true);
		$(this).closest(".fn_row").find('input[type=checkbox][name*="check"]').click();
		$(this).closest(".fn_form_list").submit();
	});

	/* Infinity in the warehouse(склад) */
	$('input[name*="stock"]').focus(function() {
		if($(this).val() == '∞')
			$(this).val('');
		return false;
	});
	$('input[name*="stock"]').blur(function() {
		if($(this).val() == '∞')
			$(this).val('∞');
	});	

	/* Delete images for products */
	if($('.images_list').size()>0){
		$('.fn_delete').on('click',function(){
			if($('.fn_accept_delete').size()>0){
				$('.fn_accept_delete').val('1');
				$(this).closest("li").fadeOut(200, function() {
					$(this).remove();
				});
			} else {
				$(this).closest("li").fadeOut(200, function() {
					$(this).remove();
				});
			}
			return false;
		});
	}

	/* Initializing sorting */
	if($("sortable").size() > 0){
		$(".sortable").each(function() {
			Sortable.create(this, {
				handle: ".move_zone",  // Drag handle selector within list items
                    sort: true,  // sorting inside list
                    animation: 150,  // ms, animation speed moving items when sorting, `0` — without animation
                    ghostClass: "sortable-ghost",  // Class name for the drop placeholder
                    chosenClass: "sortable-chosen",  // Class name for the chosen item
                    dragClass: "sortable-drag",  // Class name for the dragging item
                    scrollSensitivity: 100, // px, how near the mouse must be to an edge to start scrolling.
                    scrollSpeed: 10, // px
                    
                    // Changed sorting within list
                    onUpdate: function (evt) {
                    	if ($(".product_images_list").size() > 0) {
                            var itemEl = evt.item;  // dragged HTMLElement
                            if ($(itemEl).closest(".fn_droplist_wrap").data("image") == "product") {
                            	$(".product_images_list").find("li.first_image").removeClass("first_image");
                            	$(".product_images_list").find("li:nth-child(2)").addClass("first_image");
                            }
                         }
                      }
                   });
		});
	}

	if($(".sort_extended").size() > 0){

		// Explicitly indicate the height of the list, otherwise when the script deletes the element and puts a stub in its place, the page jumps
		$(".fn_sort_list").css('min-height', $(".fn_sort_list").outerHeight());

		$(".sort_extended").sortable({
			items: ".fn_sort_item",
			tolerance: "pointer",
			handle: ".move_zone",
			scrollSensitivity: 50,
			scrollSpeed: 100,
			scroll: true,
			opacity: 0.5,
			containment: "document",
			helper: function(event, ui) {
				if($('input[type="checkbox"][name*="check"]:checked').size() < 1) return ui;
				var helper = $('<div/>');
				$('input[type="checkbox"][name*="check"]:checked').each(function() {
					var item = $(this).closest(".fn_row");
					helper.height(helper.height() + item.innerHeight());
					if (item[0] != ui[0]){
						helper.append(item.clone());
						$(this).closest('.fn_row').remove();
					} else{
						helper.append(ui.clone());
						item.find('input[type="checkbox"][name*="check"]').attr('checked', false);
					}
				});
				return helper;
			},
			start: function(event, ui) {
				if (ui.helper.children('.fn_row').size() > 0)
					$('.ui-sortable-placeholder').height(ui.helper.height());
			},
			beforeStop: function(event, ui) {
				if(ui.helper.children('.fn_row').size() > 0){
					ui.helper.children('.fn_row').each(function() {
						$(this).insertBefore(ui.item);
					});
					ui.item.remove();
				}
			},
			update: function(event, ui) {
				$("#list_form input[name*='check']").attr('checked', false);
			}
		});
	}

	$(".fn_pagination a.droppable").droppable({
		activeClass: "drop_active",
		hoverClass: "drop_hover",
		tolerance: "pointer",
		drop: function(event, ui) {
			$(ui.helper).find('input[type="checkbox"][name*="check"]'.attr('checked', true));
			$(ui.draggable).closest("form").find('select[name="action"] option[value=move_to_page]').attr('selected', 'selected');
			$(ui.draggable).closest("form").find('select[name=target_page] option[value'+$(this).html()+']').attr('selected', 'selected');
			$(ui.draggable).closest("form").submit();
			return false;
		}
	});

	/* Call an ajax entity update */
	if($(".fn_ajax_action").size()>0){
		$(document).on("click",".fn_ajax_action",function () {
			ajax_action($(this));
		});
	}

	$(document).on('change', '.fn_action_block select.brands_action', function () {
		var elem = $(this).find('option:selected').val();
		$('.fn_hide_block').addClass('hidden');
		if ($('.fn_' + elem).size() > 0) {
			$('.fn_' + elem).removeClass('hidden');
		}
	});
});

$(document).on('click', '.fn_light_remove', function() {
	$(this).closest('.fn_row').remove();
});

if($('.fn_remove').size() > 0) {
	function success_action ($this){
		$(document).on('click','.fn_submit_delete',function(){
			$('.fn_form_list input[type="checkbox"][name*="check"]').attr('checked', false);
			$this.closest(".fn_row").find('input[type="checkbox"][name*="check"]').prop('checked', true);
			$this.closest(".fn_form_list").find('select[name="action"] option[value=delete]').prop('selected', true);
			$this.closest(".fn_form_list").submit();
		});
		$(document).on('click','.fn_dismiss_delete',function(){
			$('.fn_form_list input[type="checkbox"][name*="check"]').prop('checked', false);
			$this.closest(".fn_form_list").find('select[name="action"] option[value=delete]').removeAttr('selected');
			return false;
		});
	}
}

/* Функції генерації мета-даних */

var is_translit_alpha = $('.fn_is_translit_alpha');
var translit_pairs = [];
translit_pairs[0] =  {
	from: "А-а-Б-б-В-в-Ґ-ґ-Г-г-Д-д-Е-е-Ё-ё-Є-є-Ж-ж-З-з-И-и-І-і-Ї-ї-Й-й-К-к-Л-л-М-м-Н-н-О-о-П-п-Р-р-С-с-Т-т-У-у-Ф-ф-Х-х-Ц-ц-Ч-ч-Ш-ш-Щ-щ-Ъ-ъ-Ы-ы-Ь-ь-Э-э-Ю-ю-Я-я".split("-"),
	to: "A-a-B-b-V-v-G-g-G-g-D-d-E-e-E-e-E-e-ZH-zh-Z-z-I-i-I-i-I-i-J-j-K-k-L-l-M-m-N-n-O-o-P-p-R-r-S-s-T-t-U-u-F-f-H-h-TS-ts-CH-ch-SH-sh-SCH-sch---Y-y---E-e-YU-yu-YA-ya".split("-")
};

if($('input').is('.fn_meta_field')) {
	$(window).on("load", function() {

		// Autocomplete of meta-tags
		meta_title_touched = true;
		meta_keywords_touched = true;
		meta_description_touched = true;

		if ($('input[name = "meta_title"]').val() == generate_meta_title() || $('input[name = "meta_title"]').val() == '')
			meta_title_touched = false;
		if ($('input[name = "meta_keywords"]').val() == generate_meta_keywords() || $('input[name = "meta_keywords"]').val() == '')
			meta_keywords_touched = false;
		if ($('textarea[name = "meta_description"]').val() == generate_meta_description() || $('textarea[name = "meta_description"]').val() == '')
			meta_description_touched = false;

		$('input[name = "meta_title"]').change(function() { meta_title_touched = true; });
		$('input[name = "meta_keywords"]').change(function() { meta_keywords_touched = true; });
		$('textarea[name = "meta_description"]').change(function() { meta_description_touched = true; });

		$('#fn_meta_title_counter').text('('+ $('input[name = "meta_title"]').val().length+')');
		$('#fn_meta_description_counter').text('('+ $('textarea[name = "meta_description"]').val().replace(/\n/g, "\r\n").length+')');

		$('input[name = "name"]').keyup(function() { set_meta(); });
		$('input[name ="meta_title"]').keyup(function() { $('#fn_meta_title_counter').text('('+ $('input[name="meta_title"]').val().length+')'); });
		$('textarea[name = "meta_description"]').keyup(function() { $('#fn_meta_description_counter').text('('+ $('textarea[name = "meta_description"]').val().replace(/\n/g, "\r\n").length+')'); });

		if ($('.fn_meta_brand').size() > 0) {
			$('select[name = brand_id]').on('change', function() {
				set_meta();
			})
		}

		if ($('.fn_meta_categories').size() > 0) {
			$('.fn_meta_categories').on('change', function () {
				set_meta();
			})
		}
	});

	function set_meta() {
		if (!meta_title_touched)
			$('input[name = "meta_title"]').val(generate_meta_title());
		if (!meta_keywords_touched)
			$('input[name = "meta_keywords"]').val(generate_meta_keywords());
		if (!meta_description_touched)
			$('textarea[name = "meta_description"]').val(generate_meta_description());
		if (!$('#block_translit').is(':checked'))
			$('input[name = "url"]').val(generate_url());
	}

	function generate_meta_title() {
		name = $('input[name = "name"]').val();
		$('#fn_meta_title_counter').text('('+ name.length + ')');
		return name;
	}

	function generate_meta_keywords() {
		name = $('input[name = "name"]').val();
		result = name;

		if ($('.fn_meta_brand').size() > 0) {
			brand = $('select[name = "brand_id"] option:selected').data('brand_name');
			if (typeof(brand) == 'string' && brand != '')
				result += ', ' + brand;
		}

		if ($('.fn_meta_categories').size() > 0) {
			if($('.fn_product_categories_list .fn_category_item').size() == 0) {
				c = $('.fn_meta_categories option:selected').data('category_name');
				if (typeof(c) == 'string' && c != '')
					result += ', ' + c;
			} else {
				cat = $('.fn_product_categories_list .fn_category_item:first');
				c = cat.find('input').data('cat_name');
				if (typeof(c) == 'string' && c != '')
					result += ', ' + c;
			}
		}
		return result;
	}

	function generate_meta_description() {
		name = $('input[name = "name"]').val();
		$('#fn_meta_description_counter').text('('+ name.length + ')');
		return name;
	}
}

function generate_url() {
	url = $('input[name = "name"]').val();
	url = translit(url);
	if (is_translit_alpha.size() > 0) {
		url = url.replace(/[^0-9a-z]+/gi, '').toLowerCase();
	} else {
		url = url.replace(/[\s]+/gi, '-');
		url = url.replace(/[^0-9a-z_\-]+/gi, '').toLowerCase();
	}
	return url;
}

function translit(str) {
	var str_tm = str;
	for (var j in translit_pairs) {
		var from = translit_pairs[j].from,
		to = translit_pairs[j].to,
		res = '';
		for (var i = 0, l = str_tm.length; i < l; i++) {
			var s = str_tm.charAt(i), n = from.indexOf(s);
			if (n >= 0) {
				res += to[n];
			} else {
				res += s;
			}
		}
		str_tm = res;
	}
	return str_tm;
}

/* Функції генерації мета-даних end */

$(window).on('load', function() {

	$('#countries_select').msDropdown({
		roundedBorder: false
	});

	/* Script for tabs */

	$('.tabs').each(function(i) {
		var cur_nav = $(this).find('.tab_navigation'),
		cur_tabs = $(this).find('.tab_container');
		if(cur_nav.children('.selected').size() > 0) {
			$(cur_nav.children('.selected').attr("href")).show();
		} else {
			cur_nav.children().first().addClass('selected');
			cur_tabs.children().first().show();
		}
	});

	$('.tab_navigation_link').click(function(e){
		e.preventDefault();
		if($(this).hasClass('selected')){
			return true;
		}
		var cur_nav = $(this).closest('.tabs').find('.tab_navigation'),
		cur_tabs = $(this).closest('.tabs').find('.tab_container');
		cur_tabs.children().hide();
		cur_nav.children().removeClass('selected');
		$(this).addClass('selected');
		$($(this).attr("href")).fadeIn(200);
	});

	/* Script for tabs end */

	/* Script to collapse information blocks */
	$(document).on('click', ".fn_toggle_card", function() {
		$(this).closest(".fn_toggle_wrap").find('.fn_icon_arrow').toggleClass('rotate_180');
		$(this).closest(".fn_toggle_wrap").find('.fn_card').slideToggle(500);
	});

	/* Bloked to autoformate of link */

	$(document).on("click", ".fn_disable_url", function () {
		if($(".fn_url").attr("readonly")){
			$(".fn_url").removeAttr("readonly");
		} else {
			$(".fn_url").attr("readonly",true);
		}
		$(this).find('i').toggleClass("fa-unlock");
		$("#block_translit").trigger("click");
	});

	/* Bloked to autoformate of link end */

	if (/Android|webOs|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
		$('.selectpicker').selectpicker('mobile');
	}
});

$(document).ready(function() {
	$('.progress-bar').each(function() {
		var percentage = parseInt($(this).html());
		if (percentage > 0) {
			$(this).animate({'width':' '+ percentage + '%'}, 1800);
		} else {
			$(this).css({'color':'black', 'background': 'none'}, 4000);
		}
	})
});

/* Scripts for the drop-down lists of categories */
$(document).on('click', '.fn_ajax_toggle', function() {
	elem = $(this);
	var toggle = parseInt(elem.data("toggle"));
	var category_id = parseInt(elem.data("category_id"));
	if (toggle == 0) {
		$.ajax({
			data: {
				category_id: category_id
			},
			success: function(data) {
				var msg = "";

				if (data.success) {
					elem.closest('.fn_row').find('.fn_ajax_categories').html(data.cats);
					elem.closest('.fn_row').find('.fn_ajax_categories').addClass('sortable');
					elem.data('toggle', 1);
					elem.find('i').toggleClass('fa-minus-square');
				} else {
					toastr.erroe(msg, "Error");
				}

				var el = dicument.querySelectorAll('div.sortable , .fn_ajax_categories.sortable');
				for (i = 0; i < el.length; i++) 
				{
					var sortable = Sortable.create(el[i], {
						handle: ".move_zone",
						sort: true,
						animation: 150,
						scroll: true,
						ghostClass: "sortable-ghost",
						chosenClass: "sortable-chosen",
						dragClass: "sortable-drag",
						scrollSensitivity: 30,
						scrollSpeed: 10
					});
				}
			}
		});
	} else {
		elem.closest('.fn_row').children('.fn_ajax_categories').slideToggle(500);
		elem.find('i').toggleClass('fa-minus-square');
	}
});