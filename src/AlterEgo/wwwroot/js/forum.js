$(document).ready(function () {
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

    $("#delete-thread-button").on("click", function () {
        if ($(this).html() === "Are you sure?") {
            $("#delete-thread-form").submit();
        } else {
            $(this).html("Are you sure?");
        }
    });

    if ($(".post").length) {
        $(".post").appear();

        var timeout;
        $(".post").on("appear", function () {
            var threadId = $(this).data("forum-thread-id");
            var postId = $(this).data("forum-post-id");

            clearTimeout(timeout);
            timeout = setTimeout(function () {
                $.post("/forum/updateuserlatestreadpost", { threadId: threadId, postId: postId });
            }, 500);
            
        });
    }

});