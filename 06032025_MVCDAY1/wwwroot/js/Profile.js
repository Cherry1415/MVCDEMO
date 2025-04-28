function editProfile() {
    document.getElementById("editUserName").value = document.getElementById("userName").innerText;
    document.getElementById("editUserEmail").value = document.getElementById("userEmail").innerText;
    document.getElementById("editUserPhone").value = document.getElementById("userPhone").innerText;
   

    var modal = new bootstrap.Modal(document.getElementById("editProfileModal"));
    modal.show();
}

function saveProfileChanges() {
    document.getElementById("userName").innerText = document.getElementById("editUserName").value;
    document.getElementById("userEmail").innerText = document.getElementById("editUserEmail").value;
    document.getElementById("userPhone").innerText = document.getElementById("editUserPhone").value;

    var modal = bootstrap.Modal.getInstance(document.getElementById("editProfileModal"));
    modal.hide();
}

function deleteProfile() {
    if (confirm("Are you sure you want to delete your profile?")) {
        alert("Profile deleted successfully!");
        window.location.href = "/Home/Index";
    }
}
