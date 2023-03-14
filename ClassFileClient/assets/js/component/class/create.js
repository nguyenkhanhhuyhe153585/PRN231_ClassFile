import * as Const from "../../common/const.js";
import * as Cookies from "../../common/cookies.js";
import * as Utils from "../../common/utils.js";

export function createClass() {
  initCreateClass();
  previewSelectedImage();
  uploadImage();
  removeImage();
  doCreateClass();
}

let fileImageResponseName;

function initCreateClass() {
  let option = {};
  option.url = Const.BackEndApi.Account.My;
  option.type = Const.HttpMethod.GET;
  option.dataType = Const.HttpDataType.JSON;
  option.success = function (data) {
    render(data);
  };

  $.ajax(option);
  function render(data) {
    $("#accountAvatar").attr(
      "src",
      Utils.getUrlImage(Const.FileMode.AVATAR, data.imageAvatar)
    );
    $("#accountName").html(data.fullname);
  }
}

function previewSelectedImage() {
  $("#coverImageFile").change(function () {
    readURL(this);
  });

  function readURL(input) {
    if (input.files && input.files[0]) {
      var reader = new FileReader();

      reader.onload = function (e) {
        $("#imagePreviewCover").attr("src", e.target.result);
      };
      reader.readAsDataURL(input.files[0]);
    }
  }
}

function uploadImage() {
  let $btnUpload = $("#buttonUploadCover");
  $btnUpload
    .removeClass("btn-success")
    .addClass("btn-outline-primary")
    .html("Upload");
  $btnUpload.click(async function () {
    try {
      let fileUploadResponse = await Utils.fileUpload(
        "#coverImageFile",
        Const.FileMode.CLASS
      );
      if (fileUploadResponse) {
        $btnUpload
          .removeClass("btn-outline-primary")
          .addClass("btn-success")
          .html("Uploaded")
          .unbind("click");
        fileImageResponseName = fileUploadResponse[0].fileName;
        console.log(fileUploadResponse);
      }
    } catch (error) {
      console.log(error);
    }
  });
}

function removeImage() {
  $("#buttonRemoveCover").click(function () {
    $("#imagePreviewCover").attr("src", Const.imageHolder);
    $("#coverImageFile").val(Const.EMPTY_STRING);
    fileImageResponseName = Const.EMPTY_STRING;
    uploadImage();
  });
}

function doCreateClass() {
  $("#formCreateClass").submit(function (event) {
    event.preventDefault();

    let token = Cookies.getCookie("token");
    let payloadData = Utils.parseJwt(token);
    let teacherId = payloadData["name"];
    let teacherType = payloadData["role"];
    let className = $("#inputClassName").val();
    // let classCode = $("#inputClassCode").val();

    let opt = {};
    opt.url = Const.BackEndApi.Classes.Create;
    opt.method = Const.HttpMethod.POST;
    opt.contentType = Const.HttpDataType.ApplicationJSON;
    opt.dataType = Const.HttpDataType.JSON;
    opt.data = JSON.stringify({
      classname: className,
      accountprofile: {
        id: teacherId,
        accounttype: teacherType,
      },
      imageCover: fileImageResponseName
    });
    $.ajax(opt);
  });
}
