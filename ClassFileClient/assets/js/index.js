import * as Const from "../js/common/const.js";

export function loadClass(){
    
        let option = {};
        option.url = Const.BackEndApi.ClassesHome;
        option.type = Const.HttpMethod.GET,
        option.dataType = Const.HttpDataType.JSON;
        option.success = function (data){
            render(data);
            // console.log(data);
        }
        
        $.ajax(option);  

        function render(data){
            let result = "";
            for(let c of data){
                result += 
                `
            <div class="col">
                <div class="card h-100">
                    <img src="https://via.placeholder.com/300x150" class="card-img-top" alt="...">
                    <div class="card-img-overlay">
                        <img class="rounded-circle border img-avatar d-inline-block" src="https://via.placeholder.com/300x150" />                        
                    </div>
                    <div class="card-body">
                        <h5 class="card-title">${c.className}</h5>
                        <p class="card-text">${c.teacherAccount.fullName}</p>
                        <a class="stretched-link" href="Go to class"></a>
                    </div>
                    <div class="card-footer">
                        <small class="text-muted">Last updated 3 mins ago</small>
                    </div>
                </div>
            </div>
            `;
            }
            $("#classesList").html(result);
        }
}