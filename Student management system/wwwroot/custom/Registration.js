// global bearer token, populated on page load
let bearerToken = "";

// fetch stored token from GetToken API
async function fetchBearerToken() {
    debugger
    try {
        var urllink = "http://localhost:7214"
        var api = urllink + "/api/Token/gettoken";
        const response = await fetch(api);
        if (!response.ok) throw new Error("No token available");

        const result = await response.json();
        bearerToken = result.token; // already "Bearer eyJ..."
        console.log("Bearer token loaded.");
    } catch (err) {
        console.error("Failed to fetch bearer token:", err);
        bearerToken = "";
    }
}

// shared headers for authenticated requests
function getAuthHeaders() {
    return {
        Authorization: bearerToken,
        "Content-Type": "application/json"
    };
}

// CREATE -> POST api/StudentAPI
async function CreateStudent() {
    const data = {
        FullName: document.getElementById("FullName").value,
        Email: document.getElementById("Email").value,
        PhoneNumber: document.getElementById("PhoneNumber").value,
        Address: document.getElementById("Address").value,
        DateOfBirth: document.getElementById("DateOfBirth").value,
        Gender: document.getElementById("Gender").value,
        Course: document.getElementById("Course").value,
        UserName: document.getElementById("UserName").value,
        Password: document.getElementById("Password").value,
        ConfirmPassword: document.getElementById("ConfirmPassword").value,
        Role: document.getElementById("Role").value
    };

    try {
        const response = await fetch("/api/StudentAPI", {
            method: "POST",
            headers: getAuthHeaders(),
            body: JSON.stringify(data)
        });
        const result = await response.json();
        console.log("Create result:", result);
        return result;
    } catch (err) {
        console.error("CreateStudent failed:", err);
    }
}

// READ ALL -> GET api/StudentAPI/getstudent
async function GetAllStudents() {
    try {
        const response = await fetch("/api/StudentAPI/getstudent", {
            method: "GET",
            headers: getAuthHeaders()
        });

        if (response.status === 401 || response.status === 403) {
            console.error("Unauthorized: check token or role.");
            return null;
        }

        const result = await response.json();
        console.log("All students:", result);
        return result;
    } catch (err) {
        console.error("GetAllStudents failed:", err);
    }
}

// READ ONE -> GET api/StudentAPI/{id}
async function GetStudentById(id) {
    try {
        const response = await fetch(`/api/StudentAPI/${id}`, {
            method: "GET",
            headers: getAuthHeaders()
        });

        if (response.status === 404) {
            console.warn(`No student found with id ${id}`);
            return null;
        }

        const result = await response.json();
        console.log(`Student ${id}:`, result);
        return result;
    } catch (err) {
        console.error("GetStudentById failed:", err);
    }
}

// UPDATE -> PUT api/StudentAPI/{id}
async function UpdateStudent(id) {
    const data = {
        Id: id, // must match viewModel.Id used in controller check
        FullName: document.getElementById("FullName").value,
        Email: document.getElementById("Email").value,
        PhoneNumber: document.getElementById("PhoneNumber").value,
        Address: document.getElementById("Address").value,
        DateOfBirth: document.getElementById("DateOfBirth").value,
        Gender: document.getElementById("Gender").value,
        Course: document.getElementById("Course").value,
        UserName: document.getElementById("UserName").value,
        Password: document.getElementById("Password").value,
        ConfirmPassword: document.getElementById("ConfirmPassword").value,
        Role: document.getElementById("Role").value
    };

    try {
        const response = await fetch(`/api/StudentAPI/${id}`, {
            method: "PUT",
            headers: getAuthHeaders(),
            body: JSON.stringify(data)
        });
        const result = await response.json();
        console.log("Update result:", result);
        return result;
    } catch (err) {
        console.error("UpdateStudent failed:", err);
    }
}

// DELETE -> DELETE api/StudentAPI/{id}
async function DeleteStudent(id) {
    try {
        const response = await fetch(`/api/StudentAPI/${id}`, {
            method: "DELETE",
            headers: getAuthHeaders()
        });
        const result = await response.json();
        console.log("Delete result:", result);
        return result;
    } catch (err) {
        console.error("DeleteStudent failed:", err);
    }
}

// fetch token before page becomes interactive
document.addEventListener("DOMContentLoaded", async () => {
    await fetchBearerToken();
});