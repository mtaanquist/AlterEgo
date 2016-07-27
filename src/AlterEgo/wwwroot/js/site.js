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
        var simplemde = new SimpleMDE({
            toolbar: ["bold", "italic", "heading", "|", "quote", "image", "link", "|", "code", "|", "guide"],
            promptURLs: true
        });
    }

    // Handle panel toggle
    $('.panel-toggle').on('click', function (event) {
        event.preventDefault();
        var hpanel = $(event.target).closest('div.panel');
        var icon = $(event.target).closest('i');
        var body = hpanel.find('div.panel-body');
        var footer = hpanel.find('div.panel-footer');
        body.slideToggle(300);
        footer.slideToggle(200);

        // Toggle icon from up to down
        icon.toggleClass('fa-chevron-up').toggleClass('fa-chevron-down');
        hpanel.toggleClass('').toggleClass('panel-collapse');
        setTimeout(function () {
            hpanel.resize();
            hpanel.find('[id^=map-]').resize();
        }, 50);
    });
});