import * as Route from "./routing.js";
import * as Const from "./const.js";

// Đăng ký sự kiện Ajax
export function ajaxEvent() {
  $(document).ajaxSend(function (event, xhr, settings) {
    if (
      settings.type === Const.HttpMethod.POST ||
      settings.type === Const.HttpMethod.PUT
    )
      Swal.fire({
        title: Const.Message.Process,
        allowEscapeKey: false,
        allowOutsideClick: false,
        didOpen: () => {
          Swal.showLoading();
        },
      });
  });
  $(document).ajaxError(function (event, xhr, settings) {
    console.log(xhr.responseJSON?.data);
    let message = xhr.responseJSON?.message;
    Swal.fire({
      icon: "error",
      title: Const.Message.Oops,
      text: message,
      footer: xhr.status + " - " + xhr.statusText,
    });
  });
  $(document).ajaxComplete(function (event, xhr, settings) {
    if (xhr.status === Const.HttpCode.UnAuthorized) {
      Route.redirect(Const.Path.Login);
    }
    if (
      settings.type === Const.HttpMethod.POST ||
      settings.type === Const.HttpMethod.PUT
    ) {
      if (xhr.status === Const.HttpCode.Ok) {
        Swal.fire({
          icon: "success",
          title: Const.Message.Success,
          showConfirmButton: false,
          timer: 1500,
        });
      }
    }
  });
}
