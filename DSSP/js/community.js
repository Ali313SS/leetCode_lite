
const currentUser = localStorage.getItem("currentUser") || "john";
const users = [{"username": "john", "role": "user"}, {"username": "sara", "role": "coach"}, {"username": "rahma", "role": "coach"}, {"username": "ali", "role": "user"}, {"username": "zain", "role": "user"}];
let coachRequests = JSON.parse(localStorage.getItem("coachRequests") || "{}");

function switchTab(tabId) {
    document.querySelectorAll('.tab-content').forEach(el => el.style.display = 'none');
    document.querySelectorAll('.tabs button').forEach(el => el.classList.remove('active'));
    document.getElementById(tabId).style.display = 'block';
    document.getElementById('tab-' + tabId).classList.add('active');

    if (tabId === "find") renderFind();
    if (tabId === "myrequests") renderMyRequests();
    if (tabId === "incoming") renderIncoming();
}

function renderFind() {
    const findTab = document.getElementById("find");
    findTab.innerHTML = "<h2>Find a Coach</h2>";
    const coaches = users.filter(u => u.username !== currentUser && u.role !== "user");

    if (coaches.length === 0) {
        findTab.innerHTML += "<p>No coaches available</p>";
        return;
    }

    coaches.forEach(user => {
        const btnId = "btn-" + user.username;
        const requested = coachRequests[user.username]?.includes(currentUser);
        const div = document.createElement("div");
        div.innerHTML = `
            <strong>${user.username}</strong>
            <button id="${btnId}" onclick="sendRequest('${user.username}')" ${requested ? 'disabled' : ''}>
                ${requested ? 'Request Sent' : 'Send Request'}
            </button>
        `;
        findTab.appendChild(div);
    });
}

function sendRequest(coach) {
    if (!coachRequests[coach]) coachRequests[coach] = [];
    if (!coachRequests[coach].includes(currentUser)) {
        coachRequests[coach].push(currentUser);
        localStorage.setItem("coachRequests", JSON.stringify(coachRequests));
    }
    switchTab("find");
}

function renderMyRequests() {
    const myTab = document.getElementById("myrequests");
    myTab.innerHTML = "<h2>My Sent Requests</h2>";
    const sentTo = Object.entries(coachRequests)
        .filter(([coach, list]) => list.includes(currentUser))
        .map(([coach]) => coach);

    if (sentTo.length === 0) {
        myTab.innerHTML += "<p>You haven't sent any requests yet.</p>";
        return;
    }

    sentTo.forEach(coach => {
        const div = document.createElement("div");
        div.innerHTML = `<span>You sent a request to <strong>${coach}</strong></span>`;
        myTab.appendChild(div);
    });
}

function renderIncoming() {
    const incomingTab = document.getElementById("incoming");
    const currentUserData = users.find(u => u.username === currentUser);
    if (!currentUserData || currentUserData.role !== "coach") {
        incomingTab.innerHTML = "<p>You are not a coach.</p>";
        return;
    }

    const requests = coachRequests[currentUser] || [];
    incomingTab.innerHTML = "<h2>Incoming Requests</h2>";
    if (requests.length === 0) {
        incomingTab.innerHTML += "<p>No one requested to be coached by you.</p>";
        return;
    }

    requests.forEach(username => {
        const div = document.createElement("div");
        div.innerHTML = `<strong>${username}</strong> requested to be coached.`;
        incomingTab.appendChild(div);
    });
}

switchTab('find');
