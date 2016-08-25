$(document)
    .ready(function() {
        $("#main-character-selection tr")
            .on("click",
                function() {
                    var tableData = $(this)
                        .children("td")
                        .map(function() {
                            return $(this).text();
                        })
                        .get();

                    var characterName = tableData[0];
                    var characterRealm = tableData[4];

                    $.post("../manage/setmaincharacter", { name: characterName, realm: characterRealm })
                        .done(function() {
                            location.reload();
                        });
                });

        var url = document.location.toString();
        if (url.match("#")) {
            $('.nav-tabs a[href="#' + url.split("#")[1] + '"]').tab("show");
        }

        // Change hash for page-reload
        $(".nav-tabs a")
            .on("shown.bs.tab",
                function(e) {
                    window.location.hash = e.target.hash;
                });
    });