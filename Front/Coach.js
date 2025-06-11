// Manages coach view for Omer23: shows pending requests, accepted students, and chat
document.addEventListener("DOMContentLoaded", function () {
    const pendingSection = document.getElementById("pending-section");
    const acceptedSection = document.getElementById("accepted-section");
    const chatModalElement = document.getElementById("chatModal");
    let chatModal = null;
    try {
        if (!chatModalElement) {
            throw new Error("Chat modal element not found in DOM.");
        }
        if (!window.bootstrap) {
            throw new Error("Bootstrap JavaScript not loaded.");
        }
        chatModal = new bootstrap.Modal(chatModalElement, { backdrop: 'static', keyboard: false });
    } catch (error) {
        console.error("Modal initialization error:", error.message, error.stack);
        alert("Chat modal failed to initialize. Please ensure Bootstrap is loaded and check the console.");
    }
    const chatWithLabel = document.getElementById("chatWith");
    const chatMessages = document.getElementById("chatMessages");
    const chatInput = document.getElementById("chatInput");
    const sendMessageBtn = document.getElementById("sendMessageBtn");
    const messageToastElement = document.getElementById("messageToast");
    let messageToast = null;
    try {
        if (!window.bootstrap) {
            throw new Error("Bootstrap JavaScript not loaded.");
        }
        if (!messageToastElement) {
            throw new Error("Message toast element not found in DOM.");
        }
        messageToast = new bootstrap.Toast(messageToastElement, { delay: 3000 });
    } catch (error) {
        console.warn("Toast initialization failed:", error.message, error.stack);
        console.warn("Toast notifications will be disabled, but other features will continue to work.");
    }
    const currentUser = "Omer"; // Replace with localStorage.getItem("currentUser") for dynamic user
    const isCoach = true; // Hardcoded for coach view
    const debug = true; // Enable verbose logging
    let currentChatUser = null;

    // Load data from localStorage
    let coachRequests = JSON.parse(localStorage.getItem("coachRequests")) || {};
    let acceptedStudents = JSON.parse(localStorage.getItem("acceptedStudents")) || {};
    let userRequests = JSON.parse(localStorage.getItem("userRequests")) || {};
    let chatMessagesData = JSON.parse(localStorage.getItem("chatMessages")) || {};

    // Initialize coachRequests if missing
    if (!coachRequests[currentUser]) {
        coachRequests[currentUser] = {};
        if (debug) console.log("Initialized empty coachRequests for Omer23");
    }
    if (debug) console.log("Initial data - coachRequests:", coachRequests, "acceptedStudents:", acceptedStudents);

    // Save data to localStorage
    function saveData() {
        try {
            const data = {
                coachRequests: JSON.stringify(coachRequests),
                acceptedStudents: JSON.stringify(acceptedStudents),
                userRequests: JSON.stringify(userRequests),
                chatMessages: JSON.stringify(chatMessagesData)
            };
            const totalSize = Object.values(data).reduce((sum, str) => sum + str.length * 2, 0);
            const maxSize = 5 * 1024 * 1024; // 5MB
            if (totalSize > maxSize) {
                throw new Error(`localStorage quota exceeded: ${totalSize} bytes > ${maxSize} bytes`);
            }
            localStorage.setItem("coachRequests", data.coachRequests);
            localStorage.setItem("acceptedStudents", data.acceptedStudents);
            localStorage.setItem("userRequests", data.userRequests);
            localStorage.setItem("chatMessages", data.chatMessages);
            if (debug) {
                console.log("saveData: localStorage updated", {
                    coachRequests: localStorage.getItem("coachRequests"),
                    acceptedStudents: localStorage.getItem("acceptedStudents"),
                    userRequests: localStorage.getItem("userRequests"),
                    chatMessages: localStorage.getItem("chatMessages")
                });
            }
        } catch (error) {
            console.error("saveData error:", error.message, error.stack);
            if (error.message.includes("quota exceeded")) {
                alert("Storage limit reached. Please clear some data and try again.");
            } else if (error.message.includes("SecurityError")) {
                alert("localStorage access blocked. Please serve the page from a web server (not file://).");
            } else {
                alert("Failed to save data: " + error.message);
            }
            throw error;
        }
    }

    // Load pending requests
    function loadPendingRequests() {
        try {
            if (debug) console.log("loadPendingRequests: coachRequests =", coachRequests);
            const container = document.createElement("div");
            container.className = "row";
            const requests = coachRequests[currentUser] || {};
            const keys = Object.keys(requests);
            if (keys.length === 0) {
                container.innerHTML = `<p class="text-muted">No pending requests.</p>`;
            } else {
                keys.forEach((student) => {
                    const avatarInitial = student.charAt(0).toUpperCase();
                    const card = document.createElement("div");
                    card.className = "col-md-4";
                    card.innerHTML = `
                        <div class="card">
                            <div class="card-body">
                                <div class="avatar">${avatarInitial}</div>
                                <div class="card-content">
                                    <h5 class="card-title">Student</h5>
                                    <p class="card-text">${student}</p>
                                    <div class="card-actions">
                                        <button class="btn btn-success btn-sm accept-request" data-user="${student}">Accept</button>
                                        <button class="btn btn-danger btn-sm reject-request" data-user="${student}">Reject</button>
                                    </div>
                                </div>
                            </div>
                        </div>`;
                    container.appendChild(card);
                });
            }
            pendingSection.innerHTML = `<h2>Pending Requests</h2>`;
            pendingSection.appendChild(container);
            if (debug) console.log("loadPendingRequests: DOM updated, cards =", keys.length, ", students =", keys);
        } catch (error) {
            console.error("loadPendingRequests error:", error.message, error.stack);
            alert("Failed to load pending requests. Please refresh the page.");
        }
    }

    // Load accepted students
    function loadAcceptedStudents() {
        try {
            const container = document.createElement("div");
            container.className = "row";
            const students = acceptedStudents[currentUser] || {};
            const keys = Object.keys(students);
            if (keys.length === 0) {
                container.innerHTML = `<p class="text-muted">No accepted students.</p>`;
            } else {
                keys.forEach((student) => {
                    const avatarInitial = student.charAt(0).toUpperCase();
                    // Count unread messages from this student
                    const unreadCount = (chatMessagesData[currentUser]?.[student] || [])
                        .filter(msg => msg.sender !== currentUser && !msg.read).length;
                    const badge = unreadCount > 0 ? `<span class="badge bg-danger ms-1">${unreadCount}</span>` : "";
                    const card = document.createElement("div");
                    card.className = "col-md-4";
                    card.innerHTML = `
                        <div class="card">
                            <div class="card-body">
                                <div class="avatar">${avatarInitial}</div>
                                <div class="card-content">
                                    <h5 class="card-title">Student</h5>
                                    <p class="card-text">${student}</p>
                                    <div class="card-actions">
                                        <button class="btn btn-primary btn-sm view-profile" data-user="${student}">View Profile</button>
                                        <button class="btn btn-primary btn-sm open-chat" data-user="${student}">Chat${badge}</button>
                                        <button class="btn btn-danger btn-sm delete-student" data-user="${student}">Delete Student</button>
                                    </div>
                                </div>
                            </div>
                        </div>`;
                    container.appendChild(card);
                });
            }
            acceptedSection.innerHTML = `<h2>Accepted Students</h2>`;
            acceptedSection.appendChild(container);
            bindEventListeners();
            if (debug) console.log("loadAcceptedStudents: DOM updated, cards =", keys.length, ", students =", keys);
        } catch (error) {
            console.error("loadAcceptedStudents error:", error.message, error.stack);
            alert("Failed to load accepted students. Please refresh the page.");
        }
    }

    // Bind event consigners for dynamic buttons
    function bindEventListeners() {
        try {
            document.querySelectorAll(".accept-request").forEach(btn => {
                btn.removeEventListener("click", handleAccept);
                btn.addEventListener("click", handleAccept);
                if (debug) console.log("Bound accept-request listener for", btn.getAttribute("data-user"));
            });
            document.querySelectorAll(".reject-request").forEach(btn => {
                btn.removeEventListener("click", handleReject);
                btn.addEventListener("click", handleReject);
                if (debug) console.log("Bound reject-request listener for", btn.getAttribute("data-user"));
            });
            document.querySelectorAll(".view-profile").forEach(btn => {
                btn.removeEventListener("click", handleViewProfile);
                btn.addEventListener("click", handleViewProfile);
                if (debug) console.log("Bound view-profile listener for", btn.getAttribute("data-user"));
            });
            document.querySelectorAll(".open-chat").forEach(btn => {
                btn.removeEventListener("click", handleOpenChat);
                btn.addEventListener("click", handleOpenChat);
                if (debug) console.log("Bound open-chat listener for", btn.getAttribute("data-user"));
            });
            document.querySelectorAll(".delete-student").forEach(btn => {
                btn.removeEventListener("click", handleDeleteStudent);
                btn.addEventListener("click", handleDeleteStudent);
                if (debug) console.log("Bound delete-student listener for", btn.getAttribute("data-user"));
            });
        } catch (error) {
            console.error("bindEventListeners error:", error.message, error.stack);
            alert("Failed to bind event listeners. Please refresh the page.");
        }
    }

    // Load chat messages
    function loadChatMessages(student) {
        try {
            if (!chatMessages) {
                throw new Error("Chat messages container not found.");
            }
            chatMessages.innerHTML = "";
            const messages = chatMessagesData[currentUser]?.[student] || [];
            // Mark messages from the student as read
            if (chatMessagesData[currentUser]?.[student]) {
                const initialUnreadCount = messages.filter(msg => msg.sender !== currentUser && !msg.read).length;
                chatMessagesData[currentUser][student] = messages.map(msg => {
                    if (msg.sender !== currentUser && !msg.read) {
                        return { ...msg, read: true };
                    }
                    return msg;
                });
                saveData();
                if (debug) console.log(`loadChatMessages: Marked ${initialUnreadCount} messages from ${student} as read`);
            }
            messages.forEach((msg) => {
                const messageDiv = document.createElement("div");
                const unreadClass = msg.sender !== currentUser && !msg.read ? "unread" : "";
                messageDiv.className = `chat-message ${msg.sender === currentUser ? "sent" : "received"} ${unreadClass}`;
                messageDiv.innerHTML = `
                    <p>${msg.message}</p>
                    <small>${new Date(msg.timestamp).toLocaleString()}</small>
                `;
                chatMessages.appendChild(messageDiv);
            });
            chatMessages.scrollTop = chatMessages.scrollHeight;
            // Refresh accepted students to update unread badges
            loadAcceptedStudents();
            if (debug) console.log(`loadChatMessages: Loaded ${messages.length} messages for ${student}, badge should be updated`);
        } catch (error) {
            console.error("loadChatMessages error:", error.message, error.stack);
            alert("Failed to load chat messages. Please try again.");
        }
    }

    // Send chat message
    function sendChatMessage() {
        try {
            if (!chatInput) {
                throw new Error("Chat input element not found.");
            }
            const message = chatInput.value.trim();
            if (!message) {
                if (debug) console.log("sendChatMessage: Empty message, skipping");
                return;
            }
            if (!currentChatUser) {
                throw new Error("No chat user selected.");
            }
            if (!chatMessagesData[currentUser]) {
                chatMessagesData[currentUser] = {};
                if (debug) console.log(`sendChatMessage: Initialized chatMessagesData[${currentUser}]`);
            }
            if (!chatMessagesData[currentUser][currentChatUser]) {
                chatMessagesData[currentUser][currentChatUser] = [];
                if (debug) console.log(`sendChatMessage: Initialized chatMessagesData[${currentUser}][${currentChatUser}]`);
            }
            const timestamp = new Date().toISOString();
            const messageObj = { sender: currentUser, message, timestamp, read: false };
            chatMessagesData[currentUser][currentChatUser].push(messageObj);
            if (!chatMessagesData[currentChatUser]) {
                chatMessagesData[currentChatUser] = {};
                if (debug) console.log(`sendChatMessage: Initialized chatMessagesData[${currentChatUser}]`);
            }
            if (!chatMessagesData[currentChatUser][currentUser]) {
                chatMessagesData[currentChatUser][currentUser] = [];
                if (debug) console.log(`sendChatMessage: Initialized chatMessagesData[${currentChatUser}][${currentUser}]`);
            }
            chatMessagesData[currentChatUser][currentUser].push({ ...messageObj, read: false });
            if (debug) console.log(`sendChatMessage: Message prepared:`, messageObj);
            saveData();
            chatInput.value = "";
            loadChatMessages(currentChatUser);
            if (messageToast) {
                messageToast.show();
                if (debug) console.log("sendChatMessage: Notification toast shown");
            } else {
                console.warn("sendChatMessage: Toast not available, skipping notification");
            }
            if (debug) console.log(`sendChatMessage: Sent message to ${currentChatUser}`);
        } catch (error) {
            console.error("sendChatMessage error:", error.message, error.stack);
            alert(`Failed to send message: ${error.message}. Please check the console and try again.`);
        }
    }

    function handleAccept() {
        const user = this.getAttribute("data-user");
        if (debug) console.log(`handleAccept: Attempting to accept ${user}`);
        try {
            if (!coachRequests[currentUser] || !coachRequests[currentUser][user]) {
                throw new Error(`No pending request found for ${user}`);
            }
            if (!acceptedStudents[currentUser]) {
                acceptedStudents[currentUser] = {};
            }
            acceptedStudents[currentUser][user] = true;
            delete coachRequests[currentUser][user];
            if (Object.keys(coachRequests[currentUser]).length === 0) {
                delete coachRequests[currentUser];
            }
            if (userRequests[user] && userRequests[user][currentUser]) {
                userRequests[user][currentUser] = "accepted";
            }
            saveData();
            loadPendingRequests();
            loadAcceptedStudents();
            alert(`Accepted ${user} as a student.`);
            if (debug) console.log(`handleAccept: Accepted ${user}, DOM updated`);
        } catch (error) {
            console.error(`handleAccept error for ${user}:`, error.message, error.stack);
            alert(`Error accepting ${user}'s request: ${error.message}. Please try again.`);
        }
    }

    function handleReject() {
        const user = this.getAttribute("data-user");
        if (debug) console.log(`handleReject: Attempting to reject ${user}`);
        try {
            if (!coachRequests[currentUser] || !coachRequests[currentUser][user]) {
                throw new Error(`No pending request found for ${user}`);
            }
            delete coachRequests[currentUser][user];
            if (Object.keys(coachRequests[currentUser]).length === 0) {
                delete coachRequests[currentUser];
            }
            if (userRequests[user] && userRequests[user][currentUser]) {
                userRequests[user][currentUser] = "rejected";
            }
            saveData();
            loadPendingRequests();
            loadAcceptedStudents();
            alert(`Rejected ${user}'s request.`);
            if (debug) console.log(`handleReject: Rejected ${user}, DOM updated`);
        } catch (error) {
            console.error(`handleReject error for ${user}:`, error.message, error.stack);
            alert(`Error rejecting ${user}'s request: ${error.message}. Please try again.`);
        }
    }

    function handleViewProfile() {
        const user = this.getAttribute("data-user");
        if (debug) console.log(`handleViewProfile: Attempting to view profile for ${user}`);
        try {
            if (!user) {
                throw new Error("No user specified for profile view.");
            }
            const url = `profile.html?user=${encodeURIComponent(user)}`;
            if (debug) console.log(`Redirecting to ${url}`);
            window.location.assign(url);
        } catch (error) {
            console.error(`handleViewProfile error for ${user}:`, error.message, error.stack);
            alert("Failed to open profile page. Please ensure profile.html exists in the same directory and try again.");
        }
    }

    function handleOpenChat() {
        const user = this.getAttribute("data-user");
        if (debug) console.log(`handleOpenChat: Attempting to open chat with ${user}`);
        try {
            if (!user) {
                throw new Error("No user specified for chat.");
            }
            if (!chatModal) {
                throw new Error("Chat modal not initialized.");
            }
            currentChatUser = user;
            if (chatWithLabel) {
                chatWithLabel.textContent = user;
            } else {
                console.warn("chatWithLabel element not found, skipping label update");
            }
            loadChatMessages(user);
            chatModal.show();
            if (debug) console.log(`handleOpenChat: Chat modal opened for ${user}`);
        } catch (error) {
            console.error(`handleOpenChat error for ${user}:`, error.message, error.stack);
            alert("Failed to open chat. Please ensure Bootstrap is loaded and check the console for details.");
        }
    }

    function handleDeleteStudent() {
        const user = this.getAttribute("data-user");
        if (debug) console.log(`handleDeleteStudent: Attempting to delete ${user}`);
        try {
            if (!acceptedStudents[currentUser] || !acceptedStudents[currentUser][user]) {
                throw new Error(`No accepted student found for ${user}`);
            }
            delete acceptedStudents[currentUser][user];
            if (Object.keys(acceptedStudents[currentUser]).length === 0) {
                delete acceptedStudents[currentUser];
            }
            if (userRequests[user] && userRequests[user][currentUser]) {
                userRequests[user][currentUser] = "rejected";
            }
            saveData();
            loadAcceptedStudents();
            alert(`Deleted ${user} from your students.`);
            if (debug) console.log(`handleDeleteStudent: Deleted ${user}, DOM updated`);
        } catch (error) {
            console.error(`handleDeleteStudent error for ${user}:`, error.message, error.stack);
            alert(`Error deleting ${user}: ${error.message}. Please try again.`);
        }
    }

    // Bind send message button
    if (sendMessageBtn) {
        sendMessageBtn.addEventListener("click", sendChatMessage);
    } else {
        console.error("sendMessageBtn not found in DOM");
    }
    if (chatInput) {
        chatInput.addEventListener("keypress", (e) => {
            if (e.key === "Enter") sendChatMessage();
        });
    } else {
        console.error("chatInput not found in DOM");
    }

    // Initial load
    try {
        loadPendingRequests();
        loadAcceptedStudents();
        if (debug) console.log("Initial load completed");
    } catch (error) {
        console.error("Initial load error:", error.message, error.stack);
        alert("Failed to initialize Coach Panel. Please refresh the page.");
    }
});