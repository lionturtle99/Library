function getDetailsOfBook(bookId) {
  $(`#${bookId}-title`).click(function () {
    $("#bookTitle").show();
    $("#bookTitleInput").hide();
    $("#submitTitleChange").hide();
    $.ajax({
      type: "GET",
      url: '../../Books/Details',
      data: { 'id': bookId },
      success: function (result) {
        $("#bookTitle").text(result.thisBook.$values[0].title);
        $("#bookGenre").text(result.thisBook.$values[0].genre);
        $("#bookPages").text(result.thisBook.$values[0].pages);
        $("#bookId").val(result.thisBook.$values[0].bookId);
        $("#bookTitleInput").val(result.thisBook.$values[0].title);
        $("#listOfRenters").empty();
        for (let x = 0; x < result.listOfRenters.$values.length; x++) {

          $("#listOfRenters").append(`<li class="list-group-item"><div class="row"><div class="col-8">${result.listOfRenters.$values[x].name}</div><div class="col-4" style="font-size: 30px;"><span id="remove-${result.listOfRenters.$values[x].patronId}-from-${result.thisBook.bookId}">⊠</span></div></div></li>`);
        }
        $("#detailsCard").show();
      },
      error: function (httpObj) {
        if (httpObj.status==401) {
          alert("you aren't yet logged in");
        } else {
          alert("Error while getting book details");
        }
      }
    });
  });
}

$("#bookTitle").click(function () {
  $("#bookTitle").hide();
  $("#bookTitleInput").show();
  $("#submitTitleChange").show();
});
$("#submitTitleChange").click(function () {
  $.ajax({
    type: "POST",
    url: '../../Books/Edit',
    data: { 'bookId': $("#bookId").val(), 'title': $("#bookTitleInput").val(), 'genre': $("#bookGenre").text(), 'pages': $("#bookPages").text() },
    success: function (response) {
      $("#bookTitle").text($("#bookTitleInput").val());
      $("#bookTitleInput").hide();
      $("#submitTitleChange").hide();
      $("#bookTitle").show();
      $("#" + $("#bookId").val() + "-title").html($("#bookTitleInput").val());
      $("#bookTitleInput").val("");
      console.log(response.message);
    },
    error: function () {
      alert(`Error while updating ${$("#bookTitleInput").val()}`);
    }
  });
});

$("#delete").click(function() {
  $.ajax({
    type: "POST",
    url: '../../Books/Delete',
    data: { 'bookId':$("#bookId").val()},
    success: function (response) {
      $("#" + $("#bookId").val() + "-title").remove();
      $("#detailsCard").hide();
    },
    error: function() {
      alert(`Error while deleting ${$("#bookTitleInput").val()}`);
    }
  })
})

$("#rentButtonShowModal").click(function() {
  $("#patronList").empty();
  $.ajax({
    type: "GET",
    url: '../../Books/GetPatrons',
    success: function (response) {
      for (let x = 0; x < response.$values.length; x++) {
        $("#patronList").append(
          `<option value="${response.$values[x].patronId}">${response.$values[x].name}</option>`
        );
      }
    },
    error: function() {
      alert(`Error while fetching patrons`);
    }
  });
});

$("#rentBook").click(function() {
  $.ajax({
    type: "POST",
    url: '../../Books/RentToPatron',
    data: { 'bookId':$("#bookId").val(), 'patronId':$("#patronList").val()},
    success: function (response) {
      // $("#listOfRenters").append(`<li class="list-group-item">${response.thisPatron.name}</li>`);
      $("#listOfRenters").append(`<li class="list-group-item"><div class="row"><div class="col-8">${response.thisPatron.name}</div><div class="col-4" style="font-size: 30px;"><span id="remove-${response.thisPatron.patronId}-from-${$("#bookId").val()}">⊠</span></div></div></li>`);
      $("#rentModal").modal('hide');
    },
    error: function() {
      alert(`Error while renting ${$("#bookTitleInput").val()}`);
    }
  });
});