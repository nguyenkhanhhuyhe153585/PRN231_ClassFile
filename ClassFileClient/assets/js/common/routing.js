import * as Utils from "./utils.js";
import * as Const from "./const.js";
import * as Cookies from "./cookies.js";
export function includeHTML() {
  $("div[include-html]").each(function () {
    let each = $(this);
    let link = each.attr("include-html");
    $.ajax({
      url: link,
      method: "GET",
      global: false, // disable global ajax events
      success: function (response) {
        each.replaceWith(response);
        each.removeAttr("include-html");
      },
      error: function (response) {
        each.html("Page not found.");
      },
    });
  });
}

export function addAuthHeader(event, xhr, settings) {
  let token = Cookies.getCookie("token");
  if (token) {
    $.ajaxSetup({
      headers: {
        Authorization: `bearer ${token}`,
      },
    });
  }
}

export function getPath() {
  const currentUrl = new URL(window.location.href);
  return currentUrl.pathname;
}

export function checkPath(pathCheck) {
  const currentPath = getPath();
  return currentPath === pathCheck;
}

export function redirect(path) {
  if (!checkPath(path)) {
    window.location.replace(path);
  }
}

export function goToPage(path) {
  window.location.href = path;
}

export function back() {
  window.history.back();
}

export function setUrlParam(key, value) {
  const urlObject = new URL(window.location.href);
  urlObject.searchParams.set(key, value);
  return urlObject.toString();
}

export function getUrlParam(key) {
  const queryString = location.search;
  const urlParams = new URLSearchParams(queryString);
  return urlParams.get(key);
}

/**
 * Kiểm tra role trong token hiện tại có trùng với role truyền vào
 * @param {String} roleCheck 
 * @returns {Boolean}
 */
export function checkRole(roleCheck){
  let token = Cookies.getCookie(Const.TOKEN);
  let jwtPayload = Utils.parseJwt(token);
  return roleCheck == jwtPayload[Const.Payload.Typ];
}

export function verifyAuth() {
  let token = Cookies.getCookie("token");
  let isAnonymous = Const.PathRight.Anonymous.includes(getPath());
  if (!token && !isAnonymous) {
    redirect(Const.Path.Login);
    // return for block other action
    return false;
  }
  let payloadData = Utils.parseJwt(token);
  console.log(payloadData);

  // Init Current Info for navbar
  Utils.getCurrentUserInfo();
  // Show document after excute scripts
  $("body").show();
  return true;
}
