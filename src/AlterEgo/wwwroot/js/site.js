// Write your Javascript code.
var wowhead_tooltips = { "colorlinks": true, "iconizelinks": true, "renamelinks": true }

$(document).ready(function () {
    $("#signout-link")
        .on("click",
            function() {
                $(this).closest("form").submit();
            });

    $("abbr.timeago").timeago();
    $("img").addClass("img-responsive img-rounded");
    
    $("iframe").addClass("embed-responsive-item");
    $("iframe").parent().addClass("embed-responsive embed-responsive-16by9");

    // Handle panel toggle
    $(".panel-toggle").on("click", function (event) {
        event.preventDefault();
        var hpanel = $(event.target).closest("div.panel");
        var icon = $(event.target).closest("i");
        var body = hpanel.find("div.panel-body");
        var footer = hpanel.find("div.panel-footer");
        body.slideToggle(300);
        footer.slideToggle(200);

        // Toggle icon from up to down
        icon.toggleClass("fa-chevron-up").toggleClass("fa-chevron-down");
        hpanel.toggleClass("").toggleClass("panel-collapse");
        setTimeout(function () {
            hpanel.resize();
            hpanel.find("[id^=map-]").resize();
        }, 50);
    });
});

(function (document, history, location) {
    var HISTORY_SUPPORT = !!(history && history.pushState);

    var anchorScrolls = {
        ANCHOR_REGEX: /^#[^ ]+$/,
        OFFSET_HEIGHT_PX: 80,

        /**
         * Establish events, and fix initial scroll position if a hash is provided.
         */
        init: function () {
            this.scrollToCurrent();
            $(window).on('hashchange', $.proxy(this, 'scrollToCurrent'));
            $('body').on('click', 'a', $.proxy(this, 'delegateAnchors'));
        },

        /**
         * Return the offset amount to deduct from the normal scroll position.
         * Modify as appropriate to allow for dynamic calculations
         */
        getFixedOffset: function () {
            return this.OFFSET_HEIGHT_PX;
        },

        /**
         * If the provided href is an anchor which resolves to an element on the
         * page, scroll to it.
         * @param  {String} href
         * @return {Boolean} - Was the href an anchor.
         */
        scrollIfAnchor: function (href, pushToHistory) {
            var match, anchorOffset;

            if (!this.ANCHOR_REGEX.test(href)) {
                return false;
            }

            match = document.getElementById(href.slice(1));

            if (match) {
                anchorOffset = $(match).offset().top - this.getFixedOffset();
                $('html, body').animate({ scrollTop: anchorOffset });

                // Add the state to history as-per normal anchor links
                if (HISTORY_SUPPORT && pushToHistory) {
                    history.pushState({}, document.title, location.pathname + href);
                }
            }

            return !!match;
        },

        /**
         * Attempt to scroll to the current location's hash.
         */
        scrollToCurrent: function (e) {
            if (this.scrollIfAnchor(window.location.hash) && e) {
                e.preventDefault();
            }
        },

        /**
         * If the click event's target was an anchor, fix the scroll position.
         */
        delegateAnchors: function (e) {
            var elem = e.target;

            if (this.scrollIfAnchor(elem.getAttribute('href'), true)) {
                e.preventDefault();
            }
        }
    };

    $(document).ready($.proxy(anchorScrolls, 'init'));
})(window.document, window.history, window.location);