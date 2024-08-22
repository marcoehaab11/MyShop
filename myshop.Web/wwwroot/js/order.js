var dtble;
$(document).ready(function () {
    loaddata();
});
function loaddata(){
    dtble = $("#productsTable").DataTable({
        "ajax": {
            "url": "/Admin/Order/GetData"
        },
        "columns": [
            { "data": "id" },
            { "data": "applicationUser.name" },
            { "data": "applicationUser.phoneNumber" },
            { "data": "applicationUser.email" },
            { "data": "applicationUser.address" },
            { "data": "orderDate" },
            { "data": "shippingDate" },
            { "data": "totalPrice" },
             {
                "data": "id",
                "render": function (data) {
                    return `
                          <a href="/Admin/order/Details/${data}"  class="btn btn-warning">Details</a>
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