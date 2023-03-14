import * as Const from "./const.js";
import * as Route from "./routing.js"

/**
 * 
 * @param {string} token 
 * @returns {jsonPayload}
 */
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

/**
 * Dựa theo fileMode và fileName để lấy data image tương ứng
 * @param {string} fileMode 
 * @param {string} imageFileName 
 * @returns {string} URL
 */
export function getUrlImage(fileMode, imageFileName){
  let imageUrl = (Boolean(imageFileName))? `${Const.BackEndApi.File.Index}/${fileMode}/${imageFileName}` : Const.IMAGE_HOLDER
  return imageUrl;
}

/**
 * Paging sau khi được render sẽ được cho vào bên trong thẻ div
 * có id paginnation <div id="pagination"></div>
 * @param {PagingResponseDTO} pagingResponseData 
 */
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

/**
 * Dùng chung cho upload các file ảnh Avatar, Class cover,...
 * Thêm await trước khi gọi hàm
 * @async
 * @param {string} idFileInputElement 
 * @param {string} fileMode
 * @returns {Promise<FileDTO>}
 */
export function fileUpload(idFileInputElement, fileMode) {
  return new Promise((resolve, reject) => {
    let form_data = new FormData();
    let file_datas = $(idFileInputElement).prop("files");
    form_data.append("fileMode", fileMode);
    let totalFileSize = 0;

    for (let fdata of file_datas) {
      totalFileSize += fdata.size;
      form_data.append("file", fdata);
    }
    if(totalFileSize === 0){
      let message = Const.Message.NoFileSelect;
      Swal.fire({
        icon: "error",
        title: Const.Message.Oops,
        text: message,
      });
      reject(new Error("rejected!"));
      return;
    }
    if (totalFileSize >= Const.MaxFileSize) {
      let message = Const.Message.FileTooLarge;
      Swal.fire({
        icon: "error",
        title: Const.Message.Oops,
        text: message,
      });
      reject(new Error("rejected!"));
      return;
    }

    let option = {};
    option.url = Const.BackEndApi.File.Index;
    option.data = form_data;
    option.type = Const.HttpMethod.POST;
    option.processData = false;
    option.contentType = false;
    option.cache = false;
    option.suppressGlobalComplete = true;
    
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
