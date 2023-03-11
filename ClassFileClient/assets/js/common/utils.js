import * as Const from "./const.js";
import * as Route from "./routing.js"

export function parseJwt(token) {
  if (token) {
    let base64Url = token.split(".")[1];
    let base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
    let jsonPayload = decodeURIComponent(
      window
        .atob(base64)
        .split("")
        .map(function (c) {
          return "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2);
        })
        .join("")
    );

    return JSON.parse(jsonPayload);
  }
  return null;
}

export function pagination(pagingResponseData){
  let pagingResult = "";
    for (let i = 1; i <= pagingResponseData.totalPage; i++) {
      pagingResult += `<li class="page-item">
      <a class="page-link" href="${Route.setUrlParam(Const.PAGE, i)}">${i}</a>
      </li>`;
    }

    $("#pagination").html(`<nav aria-label="...">
    <ul class="pagination" id="pagingList">
      ${pagingResult}
    </ul>
  </nav>`);
}

// Tạm thời chưa dùng được
function fileUpload(idFileInputElement, fileMode) {
  return new Promise((resolve) => {
    let form_data = new FormData();
    let file_datas = $(idFileInputElement).prop("files");
    form_data.append("fileMode", fileMode);
    let totalFileSize = 0;

    for (let fdata of file_datas) {
      totalFileSize += fdata.size;
      form_data.append("file", fdata);
    }
    if (totalFileSize >= Const.MaxFileSize) {
      let message = Const.Message.FileTooLarge;
      Swal.fire({
        icon: "error",
        title: Const.Message.Oops,
        text: message,
      });
      return;
    }

    let option = {};
    option.url = Const.BackEndApi.File;
    option.data = form_data;
    option.type = Const.HttpMethod.POST;
    option.processData = false;
    option.contentType = false;
    option.cache = false;
    option.global = false;

    option.error = function (event, xhr, settings) {
      console.log(xhr.responseJSON);
      let message = xhr.responseJSON?.message;
      Swal.fire({
        icon: "error",
        title: Const.Message.Oops,
        text: message,
        footer: xhr.status + " - " + xhr.statusText,
      });
      if (xhr.status === Const.HttpCode.UnAuthorized) {
        Route.redirect(Const.Path.Login);
      }
    };
    option.success = function (response) {
      Swal.fire({
        icon: "success",
        title: Const.Message.Success,
        showConfirmButton: false,
        timer: 1500,
      });
      resolve(response);
    };
    $.ajax(option);
  });
}
