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

    if ($("textarea").length) {
        $("button[type=submit]").attr("disabled", "disabled");

        var simplemde = new SimpleMDE({
            forceSync: true,
            toolbar: ["bold", "italic", "heading", "|", "quote", "image", "link", "|", "code", "|", "guide"],
            promptURLs: true,
            spellChecker: false
        });

        simplemde.codemirror.on("change",
            function () {
                var subjectValid;
                var contentValid = ($("#Content").val() !== "" && $("#Content").val().length > 3);

                if ($("#Subject").length) {
                    subjectValid = $("#Subject").valid();
                } else {
                    subjectValid = true;
                }

                if (subjectValid && contentValid) {
                    $("button[type=submit]").removeAttr("disabled");
                } else {
                    $("button[type=submit]").attr("disabled", "disabled");
                }
            });
    }

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