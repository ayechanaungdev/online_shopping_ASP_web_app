function SaveAlert(position, icon, title) {
    Swal.fire({
        position: position,
        icon: icon,
        showCloseButton: true,
        title: title + ' Save Successfully',
        showConfirmButton: false,
        ConfirmButtonText: "Close",
        timer: 2000
    })
}
function EditAlert(position, icon, title) {
    Swal.fire({
        position: position,
        icon: icon,
        showCloseButton: true,
        title: title + ' update Successfully',
        showConfirmButton: false,
        ConfirmButtonText: "Close",
        timer: 2000
    })
}
function DeleteAlert(position, icon, title) {
    Swal.fire({
        position: position,
        icon: icon,
        showCloseButton: true,
        title: title + ' delete Successfully',
        showConfirmButton: false,
        ConfirmButtonText: "Close",
        timer: 2000
    })
}

function DeleteConfirm(url, id) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = url + id;
        }
    })
}

function DeliverConfirm(url,id) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delvier it!'
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = url + id;
        }
    })
}

function DeliverAlert(position, icon, title) {
    Swal.fire({
        position: position,
        icon: icon,
        showCloseButton: true,
        title: title + ' delivered Successfully',
        showConfirmButton: false,
        ConfirmButtonText: "Close",
        timer: 2000
    })
}
function ChangeAlert(position, icon, title) {
    Swal.fire({
        position: position,
        icon: icon,
        showCloseButton: true,
        title: title + ' Save Successfully',
        showConfirmButton: false,
        ConfirmButtonText: "Close",
        timer: 2000
    })
}
function WrongPasswordAlert(position, icon, title) {
    Swal.fire({
        position: position,
        icon: icon,
        showCloseButton: true,
        title: title + ' Old Password Wrong',
        showConfirmButton: false,
        ConfirmButtonText: "Close",
        timer: 2000
    })
}
/*document.getElementById('deliver').addEventListener('click', function ChangeText() {
    const btn = document.getElementById('deliver');
    if (btn.textContent == "Cancel") {
        btn.textContent = "Delivered";
    } else {
        btn.textContent = "Cancel";
    }
})*/
