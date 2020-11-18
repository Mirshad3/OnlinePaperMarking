﻿// Commom Plugins
(function($) {

	'use strict';

	// Scroll to Top Button.
	if (typeof theme.PluginScrollToTop !== 'undefined') {
		theme.PluginScrollToTop.initialize();
	}

	// Tooltips
	if ($.isFunction($.fn['tooltip'])) {
		$('[data-tooltip]:not(.manual), [data-plugin-tooltip]:not(.manual)').tooltip();
	}

	// Popover
	if ($.isFunction($.fn['popover'])) {
		$(function() {
			$('[data-plugin-popover]:not(.manual)').each(function() {
				var $this = $(this),
					opts;

				var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
				if (pluginOptions)
					opts = pluginOptions;

				$this.popover(opts);
			});
		});
	}

	// Validations
	if (typeof theme.PluginValidation !== 'undefined') {
		theme.PluginValidation.initialize();
	}

	// Match Height
	if ($.isFunction($.fn['matchHeight'])) {

		$('.match-height').matchHeight();

		// Featured Boxes
		$('.featured-boxes .featured-box').matchHeight();

		// Featured Box Full
		$('.featured-box-full').matchHeight();

	}

}).apply(this, [jQuery]);

// Animate
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginAnimate'])) {

		$(document).ready(function(){
			$(function() {

				$('[data-appear-animation]').each(function() {
					var $this = $(this),
						opts;

					var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
					if (pluginOptions)
						opts = pluginOptions;

					$this.themePluginAnimate(opts);
				});

			});
		});

	}

}).apply(this, [jQuery]);

// Carousel
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginCarousel'])) {

		$(function() {
			$('[data-plugin-carousel]:not(.manual), .owl-carousel:not(.manual):not(.owl)').each(function() {
				var $this = $(this),
					opts;

				var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
				if (pluginOptions)
					opts = pluginOptions;

				$this.themePluginCarousel(opts);
			});
		});

	}

}).apply(this, [jQuery]);

// Chart.Circular
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginChartCircular'])) {

		$(function() {
			$('[data-plugin-chart-circular]:not(.manual), .circular-bar-chart:not(.manual)').each(function() {
				var $this = $(this),
					opts;

				var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
				if (pluginOptions)
					opts = pluginOptions;

				$this.themePluginChartCircular(opts);
			});
		});

	}

}).apply(this, [jQuery]);

// Counter
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginCounter'])) {

		$(function() {
			$('[data-plugin-counter]:not(.manual), .counters [data-to]').each(function() {
				var $this = $(this),
					opts;

				var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
				if (pluginOptions)
					opts = pluginOptions;

				$this.themePluginCounter(opts);
			});
		});

	}

}).apply(this, [jQuery]);

// Double Carousel
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginDoubleCarousel'])) {

		$(function() {
			$('[data-plugin-double-carousel]:not(.manual), .double-carousel:not(.manual)').each(function() {
				var $this = $(this),
					opts;

				var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
				if (pluginOptions)
					opts = pluginOptions;

				$this.themePluginDoubleCarousel(opts);
			});
		});

	}

}).apply(this, [jQuery]);

// Float Element
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginFloatElement'])) {

		$(function() {
			$('[data-plugin-float-element]:not(.manual)').each(function() {
				var $this = $(this),
					opts;

				var pluginOptions = theme.fn.getOptionsSemicolon($this.data('plugin-options'));
				if (pluginOptions)
					opts = pluginOptions;

				$this.themePluginFloatElement(opts);
			});
		});

	}

}).apply(this, [jQuery]);

// Animated Icon
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginIcon'])) {

		$(document).ready(function(){
			$(function() {

				$('[data-icon]').each(function() {
					var $this = $(this),
						opts;

					var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
					if (pluginOptions)
						opts = pluginOptions;

					$this.themePluginIcon(opts);
				});
				
			});
		});
	}

}).apply(this, [jQuery]);

// Image Background
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginImageBackground'])) {

		$(function() {
			$('[data-plugin-image-background]:not(.manual)').each(function() {
				var $this = $(this),
					opts;

				var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
				if (pluginOptions)
					opts = pluginOptions;

				$this.themePluginImageBackground(opts);
			});
		});

	}

}).apply(this, [jQuery]);

// Lazy Load
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginLazyLoad'])) {

		$(function() {
			$('[data-plugin-lazyload]:not(.manual)').each(function() {
				var $this = $(this),
					opts;

				var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
				if (pluginOptions)
					opts = pluginOptions;

				$this.themePluginLazyLoad(opts);
			});
		});

	}

}).apply(this, [jQuery]);

// Lightbox
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginLightbox'])) {

		$(function() {
			$('[data-plugin-lightbox]:not(.manual), .lightbox:not(.manual)').each(function() {
				var $this = $(this),
					opts;

				var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
				if (pluginOptions)
					opts = pluginOptions;

				$this.themePluginLightbox(opts);
			});
		});

	}

}).apply(this, [jQuery]);

// Masonry
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginMasonry'])) {

		$(function() {
			$('[data-plugin-masonry]:not(.manual)').each(function() {
				var $this = $(this),
					opts;

				var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
				if (pluginOptions)
					opts = pluginOptions;

				$this.themePluginMasonry(opts);
			});
		});

	}

}).apply(this, [jQuery]);

// Match Height
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginMatchHeight'])) {

		$(function() {
			$('[data-plugin-match-height]:not(.manual)').each(function() {
				var $this = $(this),
					opts;

				var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
				if (pluginOptions)
					opts = pluginOptions;

				$this.themePluginMatchHeight(opts);
			});
		});

	}

}).apply(this, [jQuery]);

// Parallax
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginParallax'])) {

		$(function() {
			$('[data-plugin-parallax]:not(.manual)').each(function() {
				var $this = $(this),
					opts;

				var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
				if (pluginOptions)
					opts = pluginOptions;

				$this.themePluginParallax(opts);
			});
		});

	}

}).apply(this, [jQuery]);

// Progress Bar
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginProgressBar'])) {

		$(function() {
			$('[data-plugin-progress-bar]:not(.manual), [data-appear-progress-animation]').each(function() {
				var $this = $(this),
					opts;

				var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
				if (pluginOptions)
					opts = pluginOptions;

				$this.themePluginProgressBar(opts);
			});
		});

	}

}).apply(this, [jQuery]);

// Rmove Min Height After Load
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginRemoveMinHeight'])) {

		$(function() {
			$('[data-plugin-remove-min-height]:not(.manual)').each(function() {
				var $this = $(this),
					opts;

				var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
				if (pluginOptions)
					opts = pluginOptions;

				$this.themePluginRemoveMinHeight(opts);
			});
		});

	}

}).apply(this, [jQuery]);

// Revolution Slider
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginRevolutionSlider'])) {

		$(function() {
			$('[data-plugin-revolution-slider]:not(.manual), .slider-container .slider:not(.manual)').each(function() {
				var $this = $(this),
					opts;

				var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
				if (pluginOptions)
					opts = pluginOptions;

				$this.themePluginRevolutionSlider(opts);
			});
		});

	}

}).apply(this, [jQuery]);

// Counter
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginSliderRange'])) {

		$(function() {
			$('[data-plugin-slider-range]:not(.manual)').each(function() {
				var $this = $(this),
					opts;

				var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
				if (pluginOptions)
					opts = pluginOptions;

				$this.themePluginSliderRange(opts);
			});
		});

	}

}).apply(this, [jQuery]);

// Sort
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginSort'])) {

		$(function() {
			$('[data-plugin-sort]:not(.manual), .sort-source:not(.manual)').each(function() {
				var $this = $(this),
					opts;

				var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
				if (pluginOptions)
					opts = pluginOptions;

				$this.themePluginSort(opts);
			});
		});

	}

}).apply(this, [jQuery]);

// Steps
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginSteps'])) {

		$(function() {
			$('[data-plugin-steps]:not(.manual), .steps:not(.manual)').each(function() {
				var $this = $(this),
					opts;

				var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
				if (pluginOptions)
					opts = pluginOptions;

				$this.themePluginSteps(opts);
			});
		});

	}

}).apply(this, [jQuery]);

// Sticky
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginSticky'])) {

		$(function() {
			$('[data-plugin-sticky]:not(.manual)').each(function() {
				var $this = $(this),
					opts;

				var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
				if (pluginOptions)
					opts = pluginOptions;

				$this.themePluginSticky(opts);
			});
		});

	}

}).apply(this, [jQuery]);

// Toggle
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginToggle'])) {

		$(function() {
			$('[data-plugin-toggle]:not(.manual)').each(function() {
				var $this = $(this),
					opts;

				var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
				if (pluginOptions)
					opts = pluginOptions;

				$this.themePluginToggle(opts);
			});
		});

	}

}).apply(this, [jQuery]);

// Tweets
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginTweets'])) {

		$(function() {
			$('[data-plugin-tweets]:not(.manual)').each(function() {
				var $this = $(this),
					opts;

				var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
				if (pluginOptions)
					opts = pluginOptions;

				$this.themePluginTweets(opts);
			});
		});

	}

}).apply(this, [jQuery]);

// Video Background
(function($) {

	'use strict';

	if ($.isFunction($.fn['themePluginVideoBackground'])) {

		$(function() {
			$('[data-plugin-video-background]:not(.manual)').each(function() {
				var $this = $(this),
					opts;

				var pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
				if (pluginOptions)
					opts = pluginOptions;

				$this.themePluginVideoBackground(opts);
			});
		});

	}

}).apply(this, [jQuery]);

// Commom Partials
(function($) {

	'use strict';

	// Sticky Header
	if (typeof theme.StickyHeader !== 'undefined') {
		theme.StickyHeader.initialize();
	}

	// Sticky Secondary Header
	if (typeof theme.StickySecondaryHeader !== 'undefined') {
		theme.StickySecondaryHeader.initialize();
	}

	// Nav Menu
	if (typeof theme.Nav !== 'undefined') {
		theme.Nav.initialize();
	}

	// Search
	if (typeof theme.Search !== 'undefined') {
		theme.Search.initialize();
	}

	// Newsletter
	if (typeof theme.Newsletter !== 'undefined') {
		theme.Newsletter.initialize();
	}

	// Account
	if (typeof theme.Account !== 'undefined') {
		theme.Account.initialize();
	}

}).apply(this, [jQuery]);



$(document).ready(function () {
    var Sinhala1 = "අ.පො.ස. සාමාන්‍ය පෙළ";
    var Sinhala2 = "අ.පො.ස. උසස් පෙළ";
    var English1 = "Ordinary Level";
    var English2 = "Advanced Level";
    var Tamil1 = "மேம்பட்ட நிலை கேள்வி 1";
    var Tamil2 = "மேம்பட்ட நிலை கேள்வி 2";

    //language Selector
    $('.home-language a').click(function () {
        $('.home-language a').removeClass("active");
        $(this).addClass("active");
        $('.download-subjects-list').hide();
    });
    $('.home-language a:nth-child(1)').click(function () {
        $('.home-download-section .home-Examination-type a:nth-child(1)').html(Sinhala1);
        $('.home-download-section .home-Examination-type a:nth-child(2)').html(Sinhala2);
        $('.home-Examination-type .col-sm-6:nth-child(1) span').html(Sinhala1);
        $('.home-Examination-type .col-sm-6:nth-child(2) span').html(Sinhala2);
        $('.Subjects-languageSinhala').show();
    });
    $('.home-language a:nth-child(2)').click(function () {
        $('.home-download-section .home-Examination-type a:nth-child(1)').html(English1);
        $('.home-download-section .home-Examination-type a:nth-child(2)').html(English2);
        $('.home-Examination-type .col-sm-6:nth-child(1) span').html(English1);
        $('.home-Examination-type .col-sm-6:nth-child(2) span').html(English2);
        $('.Subjects-languageEnglish').show();
    });
    $('.home-language a:nth-child(3)').click(function () {
        $('.home-download-section .home-Examination-type a:nth-child(1)').html(Tamil1);
        $('.home-download-section .home-Examination-type a:nth-child(2)').html(Tamil2);
        $('.home-Examination-type .col-sm-6:nth-child(1) span').html(Tamil1);
        $('.home-Examination-type .col-sm-6:nth-child(2) span').html(Tamil2);
        $('.Subjects-languageTamil').show();
    });

    //Exam type Selector
    $('.home-Examination-type a').click(function () {
        $('.home-Examination-type a').removeClass("active");
        $(this).addClass("active");
    });

    //Subject Selector
    $('.download-subjects-list a').click(function () {
        $('.download-subjects-list a').removeClass("active");
        $(this).addClass("active"); 
    });


});



$(window).on('load', function () {

    //Landing past papers
    $('.owl-carousel .owl-item div').click(function () {
        var papername = $(this).text();
        $('span.pop-subject').html(papername);
        setTimeout(function () {
            $('body').addClass('pop-anim-active');
            $('.mfp-hide').addClass('popactive')
        }, 100); 

    }); 

    //Login forgot password
    $('.wr-login #headerRecover, .wr-login #headerRecoverCancel').click(function () {
        $('.wr-login-form, .recover-form').slideToggle();
    });

    //Landing past papers
    $('.mfp-hide .mfp-close').click(function () {
        $("body").find(".mfp-hide").removeClass("popactive");
        $("body").removeClass("pop-anim-active");
    }); 

    //Login button click
    $('.main-login-btn').click(function () {
        $("body").find(".signin-custom .dropdown-menu").addClass("login-click");
    }); 

});
$(document).click(function (event) { 
    if (!$(event.target).closest(".mfp-hide, .popup-image-gallery").length) {
        $("body").find(".mfp-hide").removeClass("popactive");
        $("body").removeClass("pop-anim-active");
    }

    if (!$(event.target).closest(".signin-custom").length) {
        $("body").find(".signin-custom .dropdown-menu").removeClass("login-click");
    }
});



// Slick controls
$('#popup-image-gallery').on('shown.bs.modal', function () {
    $('.popup-slider-for').slick({
        slidesToShow: 1,
        slidesToScroll: 1,
        arrows: true,
        fade: false,
        adaptiveHeight: true,
    });
});
// Slick.js: Get current and total slides (ie. 3/5)
var $status = $('.pagingInfo');
var $slickElement = $('.popup-slider-for');

$slickElement.on('init reInit afterChange', function (event, slick, currentSlide, nextSlide) {
    //currentSlide is undefined on init -- set it to 0 in this case (currentSlide is 0 based)
    var i = (currentSlide ? currentSlide : 0) + 1;
    $status.text(i + '/' + slick.slideCount);
});

// Slick slider sync situation
var slides = $(".popup-slider-for .slick-track > .slick-slide").length;
$('.popup-slider-for').on('afterChange', function (event, slick, currentSlide, nextSlide) {
    var inFocus = $('.popup-slider-for .slick-current').attr('data-slick-index');
    $('.popup-slider-nav .slick-current').removeClass('slick-current');
    $('.popup-slider-nav .slick-slide[data-slick-index="' + inFocus + '"]').trigger('click');
});


$(document).ready(function () {
    //Email Reset Password
    $('.email-reset').click(function () { //Remove this later
        emailReset();
    });
    function emailReset() {
        $('.recover-form').hide();
        $('.email-reset-code-form').slideDown();
    }

    //Mobile Reset Password
    $('.mobile-reset').click(function () { //Remove this later
        mobileReset();
    });
    function mobileReset() {
        $('.recover-form').hide();
        $('.mobile-reset-code-form').slideDown();
    }

    //Cancel Reset Password
    $('.resetScreen').click(function () { //Remove this later
        cancelReset();
    });
    function cancelReset() {
        $('.recover-form').slideDown();
        $('.email-reset-code-form').hide();
        $('.mobile-reset-code-form').hide();
        $('#frmResetPassword input').val("");
    }


    //Register Code
    $('.main-signup-btn').click(function () { //Remove this later
        registerCode();
    });
    function registerCode() {
        $('.main-registerContainer').hide();
        $('.register-Code-Container').slideDown();
    }
});