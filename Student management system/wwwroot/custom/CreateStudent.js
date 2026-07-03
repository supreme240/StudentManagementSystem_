

function CreateStudent() {

    var fullName = document.getElementById("FullName").value;
    var email = document.getElementById("Email").value;
    var phoneNumber = document.getElementById("PhoneNumber").value;
    var address = document.getElementById("Address").value;
    var dateOfBirth = document.getElementById("DateOfBirth").value;
    var gender = document.getElementById("Gender").value;
    var course = document.getElementById("Course").value;
    var userName = document.getElementById("UserName").value;
    var password = document.getElementById("Password").value;
    var confirmPassword = document.getElementById("ConfirmPassword").value;
    var role = document.getElementById("Role").value;


    var data = {
        FullName: fullName,
        Email: email,
        PhoneNumber: phoneNumber,
        Address: address,
        DateOfBirth: dateOfBirth,
        Gender: gender,
        Course: course,
        UserName: userName,
        Password: password,
        ConfirmPassword: confirmPassword,
        Role: role
    };
    console.log(data, "test data");

    fetch("/StudentAPI/Create", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(data)
    })
        .then(function (response) {
            return response.json();
        })
        .then(function (result) {
            console.log(result);
        });
}