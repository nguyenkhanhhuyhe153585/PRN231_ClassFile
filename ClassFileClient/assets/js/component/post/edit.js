import * as Const from "../../common/const.js";
import * as Route from "../../common/routing.js";

export function editPost() {
  initEditPostData();
}

async function initEditPostData() {
  const postId = Route.getUrlParam(Const.ID_PARAM);

  let option = {};
  option.url = Const.BackEndApi.Post + `/${postId}`;
  option.type = Const.HttpMethod.GET;
  option.dataType = Const.HttpDataType.JSON;
  option.success = function (response) {
    render(response).then(initDeleteFile);
  };

  async function render(data) {
    $("#contentPostTextArea").val(data.title);
    console.log(data);
    let result = "";
    for (let f of data.files) {
      result += `
            <tr>
                <td class="fileName"><a href="${Const.BackEndApi.File}/${f.fileName}" data-fileId="${f.id}">${f.fileNameRoot}</a></td>
                <td class="text-end btnRow"><a href="#" class="btnRemoveFile text-danger">Remove</a></td>
            </tr>
            `;
    }
    $("#tableFilePreview").html(result);
  }

  $.ajax(option);
}

function initDeleteFile() {
  $("a.btnRemoveFile").click(function (event) {
    event.preventDefault();
    let fileId = $(this)
      .parent()
      .parent()
      .children(".fileName")
      .children()
      .attr("data-fileId");
    if (fileId) {
      console.log(fileId);
      deleteFile(fileId);
    }
  });
}

function deleteFile(fileId) {
  const postId = Route.getUrlParam(Const.ID_PARAM);

  let option = {};
  option.url = Const.BackEndApi.File + `/${postId}/${fileId}`;
  option.type = Const.HttpMethod.DELETE;
  option.success = function(){
    initEditPostData();
  }
  $.ajax(option);
}
