import * as Const from "../../common/const.js";
import * as Route from "../../common/routing.js";
import * as Utils from "../../common/utils.js";

let fileImageResponseName;

export function editProfile() {
  initEditInfo();
  previewSelectedImage();
  removeImage();
  uploadImage();
  changeImage();
  editInfo();
  editPassword();
}

function previewSelectedImage() {
  $("#avatarImgFile").change(function () {
    readURL(this);
  });

  function readURL(input) {
    if (input.files && input.files[0]) {
      var reader = new FileReader();

      reader.onload = function (e) {
        $("#imagePreviewAvatar").attr("src", e.target.result);
      };
      reader.readAsDataURL(input.files[0]);
    }
  }
}

function uploadImage() {
  let $btnUpload = $("#buttonUploadAvatar");
  $btnUpload
    .removeClass("btn-success")
    .addClass("btn-outline-primary")
    .html("Upload");
  $btnUpload.click(async function () {
    try {
      let fileUploadResponse = await Utils.fileUpload(
        "#avatarImgFile",
        Const.FileMode.AVATAR
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

function changeImage() {
  $("#avatarImgFile").change(uploadImage);
}

function removeImage() {
  $("#buttonRemoveAvatar").click(function () {
    $("#imagePreviewAvatar").attr("src", Const.imageHolder);
    $("#avatarImgFile").val(Const.EMPTY_STRING);
    fileImageResponseName = Const.EMPTY_STRING;
    uploadImage();
  });
}

function initEditInfo() {
  let option = {};
  option.url = Const.BackEndApi.Account.My;
  option.type = Const.HttpMethod.GET;
  option.dataType = Const.HttpDataType.JSON;
  option.success = function (data) {
    render(data);
  };

  $.ajax(option);
  function render(data) {
    console.log(data);
    $("#imagePreviewAvatar").attr("src", Utils.getUrlImage(data.imageAvatar));

    $("#inputUsername").attr("placeholder", data.username);
    $("#inputFullName").attr("placeholder", data.fullname);

    $("#inputUsername").val(data.username);
    $("#inputFullName").val(data.fullname);
  }
}

function editInfo() {
  $("#formEditProfile").submit(function (event) {
    event.preventDefault();

    let username = $("#inputUsername").val();
    let fullname = $("#inputFullName").val();
    
    let form = new FormData();
    form.append("username", username);
    form.append("fullname", fullname);
    if (fileImageResponseName != undefined) {
      form.append("imageAvatar", fileImageResponseName);;
    }

    let option = {};
    option.url = Const.BackEndApi.Account.Edit;
    option.type = Const.HttpMethod.PUT;
    option.processData = false;
    option.contentType = false;
    option.data = form;

    $.ajax(option);
  });
}

function editPassword() {
  $("#formEditPassword").submit(function (event) {
    event.preventDefault();

    let oldPassword = $("#inputOldPassword").val();
    let newPassword = $("#inputNewPassword").val();
    let cfPassword = $("#inputConfirmPassword").val();

    let dataRequest = {
      oldPassword: oldPassword,
      newPassword: newPassword,
      confirmPassword: cfPassword,
    };

    let option = {};
    option.url = Const.BackEndApi.Account.EditPassword;
    option.type = Const.HttpMethod.PUT;
    option.contentType = Const.HttpDataType.ApplicationJSON;
    option.dataType = Const.HttpDataType.JSON;
    option.data = JSON.stringify(dataRequest);
    $.ajax(option);
  });
}
