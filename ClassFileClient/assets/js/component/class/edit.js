import * as Const from "../../common/const.js";
// import * as Cookies from "../../common/cookies.js";
import * as Utils from "../../common/utils.js";
import * as Route from "../../common/routing.js";

export function editClass() {
    initCurrentUser();
    initEditClass();
    previewSelectedImage();
    uploadImage();
    removeImage();
    doEditClass();
}

let fileImageResponseName;
let fileImageEdit;

function initCurrentUser() {
    let option = {};
    option.url = Const.BackEndApi.Account.My;
    option.type = Const.HttpMethod.GET;
    option.dataType = Const.HttpDataType.JSON;
    option.success = function (data) {
        render(data);
    };

    $.ajax(option);
    function render(data) {
        $("#accountAvatarEdit").attr(
        "src",
        Utils.getUrlImage(Const.FileMode.AVATAR, data.imageAvatar)
        );
        $("#accountNameEdit").html(data.fullname);
    }
}

function initEditClass() {
    let classId = Route.getUrlParam("id");
    let option = {};
    option.url = Const.BackEndApi.Classes.Home + `/${classId}`;
    option.type = Const.HttpMethod.GET;
    option.dataType = Const.HttpDataType.JSON;
    option.success = function (data) {
        render(data);
    };

    $.ajax(option);
    function render(data) {
        $("#imagePreviewCoverEdit").attr(
        "src",
        Utils.getUrlImage(Const.FileMode.CLASS, data.imageCover)
        );
        $("#inputClassNameEdit").attr("value", data.className);
        fileImageEdit = data.imageCover;
    }
}

function previewSelectedImage() {
    $("#coverImageFileEdit").change(function () {
      readURL(this);
    });
  
    function readURL(input) {
      if (input.files && input.files[0]) {
        var reader = new FileReader();
  
        reader.onload = function (e) {
          $("#imagePreviewCoverEdit").attr("src", e.target.result);
        };
        reader.readAsDataURL(input.files[0]);
      }
    }
}

function uploadImage() {
    let $btnUpload = $("#buttonUploadCoverEdit");
    $btnUpload
      .removeClass("btn-success")
      .addClass("btn-outline-primary")
      .html("Upload");
    $btnUpload.click(async function () {
      try {
        let fileUploadResponse = await Utils.fileUpload(
          "#coverImageFileEdit",
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
    $("#buttonRemoveCoverEdit").click(function () {
      $("#imagePreviewCoverEdit").attr("src", Const.imageHolder);
      $("#coverImageFileEdit").val(Const.EMPTY_STRING);
      fileImageResponseName = Const.EMPTY_STRING;
      uploadImage();
    });
}

function doEditClass() {
    $("#formEditClass").submit(function (event) {
      event.preventDefault();
      let className = $("#inputClassNameEdit").val();
  
      let classId = Route.getUrlParam("id");
      let opt = {};
      opt.url = Const.BackEndApi.Classes.Edit + `/${classId}`;
      opt.method = Const.HttpMethod.PUT;
      opt.contentType = Const.HttpDataType.ApplicationJSON;
      opt.suppressGlobalComplete = true;
      opt.data = JSON.stringify({
        id: classId,
        classname: className,
        imageCover: Boolean(fileImageResponseName) ? fileImageResponseName : fileImageEdit
      });
      opt.success = function() {
        Swal.fire({
          icon: "success",
          title: Const.Message.Success,
          showConfirmButton: false,
          timer: 1500,
        }).then(Route.back);
      }
      $.ajax(opt);
    });

    $("#cancelEdit").click(function(event) {
        event.preventDefault();
        let opt = {};
        opt.success = Route.back;
        $.ajax(opt);
    });
}