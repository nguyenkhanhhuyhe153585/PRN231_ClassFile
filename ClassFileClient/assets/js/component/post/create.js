import * as Route from "../../common/routing.js";
import * as Const from "../../common/const.js";

export function initCreatePost() {
  let classId = Route.getUrlParam("classId");
  let option = {};
  option.url = Const.BackEndApi.Classes.Home + `/${classId}`;
  option.type = Const.HttpMethod.GET;
  option.dataType = Const.HttpDataType.JSON;
  option.success = function (data) {
    render(data);
  };

  $.ajax(option);

  function render(data) {
    $("#classNameTitle").html(data.className);
    $("head title", window.parent.document).text("Post: " + data.className);
  }
}

export function createPost() {
  $("#formCreatePost").submit(function (event) {
    event.preventDefault();
    let form_data = new FormData();

    let classId = Route.getUrlParam("classId");
    let contentText = $("#contentPostTextArea").val();
    let file_datas = $("#fileUploadChoose").prop("files");

    form_data.append("classId", classId);
    form_data.append("content", contentText);
    form_data.append("fileMode", Const.FileMode.POST);

    let totalFileSize = 0;

    for (let fdata of file_datas) {
      totalFileSize += fdata.size;
      
      form_data.append("file", fdata);
    }
    if(totalFileSize >= Const.MaxFileSize){
      let message = Const.Message.FileTooLarge;
      Swal.fire({
        icon: "error",
        title: Const.Message.Oops,
        text: message,
      });
      return;
    }

    let option = {};
    option.url = Const.BackEndApi.Post;
    option.data = form_data;
    option.type = Const.HttpMethod.POST;
    option.processData = false;
    option.contentType = false;
    option.cache = false;
    option.success = function (response) {
      history.back();
    };

    $.ajax(option);
  });
}
