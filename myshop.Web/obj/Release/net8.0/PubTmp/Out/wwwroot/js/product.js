var dtble;
$(document).ready(function () {
    loaddata();
});
function loaddata(){
    dtble = $("#productsTable").DataTable({ 
        "ajax": {
            "url":"/Admin/Product/GetData"
        },
        "columns": [
            { "data": "name" },
            { "data": "description" },
            { "data": "price" },
            { "data": "category.name" },
            {
                "data": "id",
                "render": function (data) {
                    return `

                          <a href="/Admin/Product/Edit/${data}"  class="btn btn-success">Edit</a>
                        <a onClick=DeleteItem("/Admin/Product/Delete/${data}") class="btn btn-danger">Delete</a>
                    `
                }
            }

        ]
    });
};


function DeleteItem(url2) {
    const swalWithBootstrapButtons = Swal.mixin({
        customClass: {
            confirmButton: "btn btn-success",
            cancelButton: "btn btn-danger"
        },
        buttonsStyling: false
    });
    swalWithBootstrapButtons.fire({
        title: "Are you sure?",
        text: "You Want to Delete this Product ?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "No, cancel!",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed)
        {
           
            $.ajax({
                url: url2,
                type: "Delete",
                success: function (data) {
                    if (data.success)
                    {
                        dtble.ajax.reload();
                        swalWithBootstrapButtons.fire
                            ({
                            title: "Deleted!",
                            text: "Your file has been deleted.",
                            icon: "success"
                            });
                        
                    }
                    else
                    {
                        toaster.error(data.message);
                    }

                }

            })
           
        }
        else if (
            /* Read more about handling dismissals below */
            result.dismiss === Swal.DismissReason.cancel
        ) {
            swalWithBootstrapButtons.fire({
                title: "Cancelled",
                text: "Your Product file is safe :)",
                icon: "error"
            });
        }
    });
}