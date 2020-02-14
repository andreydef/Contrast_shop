$(function() {

	/* Initializing the scrollbar */
	if($('.scrollvar-inner').size() > 0){
		$('.scrollbar-inner').scrollbar({
			"disableBodyScroll": true
		});
	}

	if($(window).width() < 1199){
		if($('.scrollbar-variant').size() > 0){
			$('.scrollvar-variant').scrollbar();
		}
	}

	if($('form.fn_fast_button').size() > 0){

		// Зв'язка селекторів масових дій
		$(document).on('change', '.fn_action_block:not(.fn_fast_action_block) select', function(e, trigger){
			if(!trigger){
				var name = $(this).attr('name'),
				selected = $(this).children(':selected').val();
				$('.fn_fast_save select[name="' + name + '"]').val(selected).trigger('change', {trigger: true});
			}
		});

		$(document).on('change', '.fn_fast_save select', function(e, trigger){
			if(!trigger){
				var name = $(this).attr('name'),
				selected = $(this).children(':selected').val();
				$('form.fn_fast_button select[name="' + name + '"]').val(selected).trigger('change', {trigger: true});
			}
		});

		if($('.fn_action_block').size() > 0){
			var action_block = $('.contrast_list_option').clone(true);
			$('.fn_fast_action_block .action').html(action_block);

			if($('.fn_additional_params').size()){
				var additional_params = $('.fn_additional_params').clone(true);
				$('.fn_fast_action_block .additional_params').html(additional_params);
			}
		}

		$('input, textarea, select, .dropdown-toggle .fn_sort_item, .fn_category_item').bind('keyup change dragover click', function(){
			$('.fn_fast_save').show();
		});

		$('.fn_fast_save .fast_save_button').on('click', function() {
			$('body').find("form.fn_fast_button").trigger('submit');
		});
	}
	/* Initializing the scrollbar end*/


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

	/* Script to collapse information blocks */
	$(document).on('click', ".fn_toggle_card", function() {
		$(this).closest(".fn_toggle_wrap").find('.fn_icon_arrow').toggleClass('rotate_180');
		$(this).closest(".fn_toggle_wrap").find('.fn_card').slideToggle(500);
	});
});