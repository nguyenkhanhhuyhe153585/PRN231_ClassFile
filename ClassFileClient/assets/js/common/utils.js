import * as Const from "./const.js";
import { getCookie } from "./cookies.js";
import * as Route from "./routing.js";

/**
 * Format date dạng string truyền vào thành datetime theo định dạng local
 * @param {string} dateString
 * @returns {string} Datetime with format
 */
export function formatDate(dateString) {
  if (dateString) {
    let dateTime = new Date(dateString);
    // return dateTime.toLocaleDateString("vi-VI");
    return dateTime.toLocaleString();
  }
  return "???";
}

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
export function getUrlImage(fileMode, imageFileName) {
  let imageUrl = Boolean(imageFileName)
    ? `${Const.BackEndApi.File.Index}/${fileMode}/${imageFileName}`
    : Const.IMAGE_HOLDER;
  return imageUrl;
}

/**
 * Paging sau khi được render sẽ được cho vào bên trong thẻ div
 * có id paginnation <div id="pagination"></div>
 * @param {PagingResponseDTO} pagingResponseData
 */
export function pagination(pagingResponseData) {
  console.log(pagingResponseData);
  let pagingResult = "";
  let gaps = 2;
  let pageIndex = pagingResponseData.pageIndex;

  pagingResult += `
  <li class="page-item">
  <a class="page-link" href="${Route.setUrlParam(Const.PAGE, 1)}">First</a>
</li>`;
  for (let i = pageIndex - gaps; i < pageIndex; i++) {
    if (i > 0)
      pagingResult += `
    <li class="page-item">
      <a class="page-link" href="${Route.setUrlParam(Const.PAGE, i)}">${i}</a>
    </li>`;
  }
  pagingResult += `
     <li class="page-item">
       <a class="page-link active">${pageIndex}</a>
     </li>`;
  for (let i = pageIndex + 1; i <= pageIndex + gaps; i++) {
    if (i <= pagingResponseData.totalPage)
      pagingResult += `
      <li class="page-item">
        <a class="page-link" href="${Route.setUrlParam(Const.PAGE, i)}">${i}</a>
      </li>`;
  }
  pagingResult += `
  <li class="page-item">
  <a class="page-link" href="${Route.setUrlParam(
    Const.PAGE,
    pagingResponseData.totalPage
  )}">Last</a>
</li>`;

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
    if (totalFileSize === 0) {
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
/**
 * Kiểm tra role trong token hiện tại có trùng với role truyền vào
 * @param {String} roleCheck
 * @returns {Boolean}
 */
export function checkRole(roleCheck) {
  let token = getCookie(Const.TOKEN);
  let jwtPayload = parseJwt(token);
  return roleCheck == jwtPayload[Const.Payload.Typ];
}

/**
 * Kiểm tra id người dùng được truyền vào có trùng với id người dùng đang đăng nhập không
 * @param {int} userId
 * @returns {boolean}
 */
export function checkUser(userId) {
  let token = getCookie(Const.TOKEN);
  let jwtPayload = parseJwt(token);
  let currentUserId = jwtPayload[Const.Payload.Name];
  return userId == currentUserId;
}

/**
 * Lấy thông tin người dùng hiện tại và điền vào các thẻ tương ứng trong screen
 *
 * @returns {AccountProfileDTO} data
 */
export function getCurrentUserInfo() {
  return new Promise(function (resolve) {
    let option = {};
    option.url = Const.BackEndApi.Account.My;
    option.type = Const.HttpMethod.GET;
    option.dataType = Const.HttpDataType.JSON;

    option.success = function (data) {
      $(".userName").html(data.fullname);
      $("img.userAvatar").attr(
        "src",
        getUrlImage(Const.FileMode.AVATAR, data.imageAvatar)
      );
      resolve(data);
    };
    $.ajax(option);
  });
}
