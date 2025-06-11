
// Helper to get current logged-in user (simulate login)
const currentUser = localStorage.getItem("currentUser") || "sara"; // e.g. sara is a coach

// Load pending coach requests from localStorage
const coachRequests = JSON.parse(localStorage.getItem("coachRequests") || "{}");

// Get or create list of students for current coach
const coachStudents = JSON.parse(localStorage.getItem("coachStudents") || "{}");

const requestSection = document.getElementById("request-section");
const studentsSection = document.getElementById("students-section");

function renderRequests() {
    requestSection.innerHTML = "<h2>Requests to be coached by you:</h2>";
    const requests = coachRequests[currentUser] || [];

    if (requests.length === 0) {
        requestSection.innerHTML += "<p>No pending requests.</p>";
        return;
    }

    requests.forEach((username, index) => {
        const div = document.createElement("div");
        div.innerHTML = `
            <span>${username}</span>
            <button onclick="acceptRequest('${username}')">Accept</button>
            <button onclick="rejectRequest(${index})">Reject</button>
        `;
        requestSection.appendChild(div);
    });
}

function acceptRequest(username) {
    // Add student to coachStudents
    if (!coachStudents[currentUser]) coachStudents[currentUser] = [];
    coachStudents[currentUser].push(username);
    localStorage.setItem("coachStudents", JSON.stringify(coachStudents));

    // Remove from requests
    coachRequests[currentUser] = coachRequests[currentUser].filter(u => u !== username);
    localStorage.setItem("coachRequests", JSON.stringify(coachRequests));

    renderRequests();
    renderStudents();
}

function rejectRequest(index) {
    coachRequests[currentUser].splice(index, 1);
    localStorage.setItem("coachRequests", JSON.stringify(coachRequests));
    renderRequests();
}

function renderStudents() {
    studentsSection.innerHTML = "<h2>Your Students:</h2>";
    const students = coachStudents[currentUser] || [];

    if (students.length === 0) {
        studentsSection.innerHTML += "<p>You have no students yet.</p>";
        return;
    }

    students.forEach(username => {
        const div = document.createElement("div");
        div.innerHTML = `<strong>${username}</strong> <button>View Profile</button>`;
        studentsSection.appendChild(div);
    });
}

renderRequests();
renderStudents();
