$(document).ready(function() {
    $("#search-button").click(function() {
        $.ajax({
            url: "http://localhost:5000/api/person/1"
        }).done(function(data) {
            $("#search-results").text(data);
        });
    });
});