document.addEventListener("DOMContentLoaded", function () {
    const sendRequestSection = document.getElementById("send-request-section");
    const incomingRequestsSection = document.getElementById("incoming-requests-section");
    const sentRequestsSection = document.getElementById("sent-requests-section");
    const requestCoachBtn = document.getElementById("requestCoachBtn");
    const requestCoachModalElement = document.getElementById("requestCoachModal");
    const coachNameInput = document.getElementById("coachNameInput");
    const usernameSuggestions = document.getElementById("usernameSuggestions");
    const submitCoachRequest = document.getElementById("submitCoachRequest");
    let requestCoachModal = null;
    try {
        if (!requestCoachModalElement) {
            throw new Error("Coach request modal element not found in DOM.");
        }
        if (!window.bootstrap) {
            throw new Error("Bootstrap JavaScript not loaded.");
        }
        requestCoachModal = new bootstrap.Modal(requestCoachModalElement, { backdrop: 'static', keyboard: false });
    } catch (error) {
        console.error("Coach request modal initialization error:", error.message, error.stack);
        alert("Coach request modal failed to initialize. Please ensure Bootstrap is loaded.");
    }
    const chatModalElement = document.getElementById("chatModal");
    let chatModal = null;
    try {
        if (!chatModalElement) {
            throw new Error("Chat modal element not found in DOM.");
        }
        chatModal = new bootstrap.Modal(chatModalElement, { backdrop: 'static', keyboard: false });
    } catch (error) {
        console.error("Chat modal initialization error:", error.message, error.stack);
        alert("Chat modal failed to initialize. Please ensure Bootstrap is loaded.");
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
    const currentUser = "Basant"; // Replace with localStorage.getItem("currentUser") for dynamic user
    const isCoach = false; // Hardcoded for non-coach view
    const debug = true; // Enable verbose logging
    let currentChatUser = null;

    // Load data from localStorage
    let coachRequests = JSON.parse(localStorage.getItem("coachRequests")) || {};
    let userRequests = JSON.parse(localStorage.getItem("userRequests")) || {};
    let chatMessagesData = JSON.parse(localStorage.getItem("chatMessages")) || {};
    let acceptedStudents = JSON.parse(localStorage.getItem("acceptedStudents")) || {};
    let allUsers = JSON.parse(localStorage.getItem("allUsers")) || ["Ali", "Omer", "Reem", "Aya"];
    localStorage.setItem("allUsers", JSON.stringify(allUsers));
    if (debug) console.log("allUsers initialized:", allUsers);

    function saveData() {
        try {
            const data = {
                coachRequests: JSON.stringify(coachRequests),
                acceptedStudents: JSON.stringify(acceptedStudents),
                userRequests: JSON.stringify(userRequests),
                chatMessages: JSON.stringify(chatMessagesData),
                allUsers: JSON.stringify(allUsers)
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
            localStorage.setItem("allUsers", data.allUsers);
            if (debug) {
                console.log("saveData: localStorage updated", {
                    userRequests: localStorage.getItem("userRequests"),
                    acceptedStudents: localStorage.getItem("acceptedStudents"),
                    allUsers: localStorage.getItem("allUsers")
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

    function loadIncomingRequests(attempt = 1, maxAttempts = 3) {
        try {
            coachRequests = JSON.parse(localStorage.getItem("coachRequests")) || {};
            if (debug) console.log(`loadIncomingRequests (attempt ${attempt}): coachRequests =`, coachRequests);
            const container = document.createElement("div");
            container.className = "row";
            const requests = coachRequests[currentUser] || {};
            const keys = Object.keys(requests);
            if (keys.length === 0 && attempt < maxAttempts) {
                if (debug) console.log(`loadIncomingRequests: No requests found, retrying (${attempt}/${maxAttempts})`);
                setTimeout(() => loadIncomingRequests(attempt + 1, maxAttempts), 100);
                return;
            }
            if (keys.length === 0) {
                container.innerHTML = `<p class="text-muted">No incoming requests.</p>`;
            } else {
                keys.forEach((student) => {
                    const avatarInitial = student.charAt(0).toUpperCase();
                    const status = (userRequests[student] && userRequests[student][currentUser]) || "pending";
                    const card = document.createElement("div");
                    card.className = "col-md-4";
                    card.innerHTML = `
                        <div class="card">
                            <div class="card-body">
                                <div class="avatar">${avatarInitial}</div>
                                <div class="card-content">
                                    <h5 class="card-title">Student</h5>
                                    <p class="card-text">${student}</p>
                                    <p class="card-text status-${status}">Status: ${status.charAt(0).toUpperCase() + status.slice(1)}</p>
                                </div>
                            </div>
                        </div>`;
                    container.appendChild(card);
                });
            }
            incomingRequestsSection.innerHTML = `<h2>Incoming Requests</h2>`;
            incomingRequestsSection.appendChild(container);
            if (debug) console.log(`loadIncomingRequests: DOM updated, cards = ${keys.length}, attempt = ${attempt}`);
        } catch (error) {
            console.error(`loadIncomingRequests error (attempt ${attempt}):`, error.message, error.stack);
            if (attempt < maxAttempts) {
                setTimeout(() => loadIncomingRequests(attempt + 1, maxAttempts), 100);
            } else {
                alert("Failed to load incoming requests. Please refresh the page.");
            }
        }
    }

    function loadSentRequests(attempt = 1, maxAttempts = 3) {
        try {
            userRequests = JSON.parse(localStorage.getItem("userRequests")) || {};
            if (debug) console.log(`loadSentRequests (attempt ${attempt}): userRequests =`, userRequests);
            const container = document.createElement("div");
            container.className = "row";
            const requests = userRequests[currentUser] || {};
            const keys = Object.keys(requests);
            if (keys.length === 0 && attempt < maxAttempts) {
                if (debug) console.log(`loadSentRequests: No requests found, retrying (${attempt}/${maxAttempts})`);
                setTimeout(() => loadSentRequests(attempt + 1, maxAttempts), 100);
                return;
            }
            if (keys.length === 0) {
                container.innerHTML = `<p class="text-muted">No sent requests.</p>`;
            } else {
                keys.forEach((coach) => {
                    const avatarInitial = coach.charAt(0).toUpperCase();
                    const status = requests[coach];
                    const unreadCount = (chatMessagesData[currentUser]?.[coach] || [])
                        .filter(msg => msg.sender !== currentUser && !msg.read).length;
                    const badge = unreadCount > 0 ? `<span class="badge bg-danger ms-1">${unreadCount}</span>` : "";
                    const card = document.createElement("div");
                    card.className = "col-md-4";
                    let buttons = "";
                    if (status === "accepted") {
                        buttons = `
                            <button class="btn btn-primary btn-sm view-profile" data-user="${coach}">View Profile</button>
                            <button class="btn btn-primary btn-sm open-chat" data-user="${coach}">Chat${badge}</button>
                            <button class="btn btn-danger btn-sm delete-coach" data-user="${coach}">Delete Coach</button>`;
                    } else if (status === "pending") {
                        buttons = `<button class="btn btn-danger btn-sm delete-coach" data-user="${coach}">Delete Request</button>`;
                    }
                    card.innerHTML = `
                        <div class="card">
                            <div class="card-body">
                                <div class="avatar">${avatarInitial}</div>
                                <div class="card-content">
                                    <h5 class="card-title">Coach</h5>
                                    <p class="card-text">${coach}</p>
                                    <p class="card-text status-${status}">Status: ${status.charAt(0).toUpperCase() + status.slice(1)}</p>
                                    <div class="card-actions">
                                        ${buttons}
                                    </div>
                                </div>
                            </div>
                        </div>`;
                    container.appendChild(card);
                });
            }
            sentRequestsSection.innerHTML = `<h2>Your Sent Requests</h2>`;
            sentRequestsSection.appendChild(container);
            document.querySelectorAll(".open-chat").forEach(btn => {
                btn.removeEventListener("click", handleOpenChat);
                btn.addEventListener("click", handleOpenChat);
                if (debug) console.log("Bound open-chat listener for", btn.getAttribute("data-user"));
            });
            document.querySelectorAll(".delete-coach").forEach(btn => {
                btn.removeEventListener("click", handleDeleteCoach);
                btn.addEventListener("click", handleDeleteCoach);
                if (debug) console.log("Bound delete-coach listener for", btn.getAttribute("data-user"));
            });
            document.querySelectorAll(".view-profile").forEach(btn => {
                btn.removeEventListener("click", handleViewProfile);
                btn.addEventListener("click", handleViewProfile);
                if (debug) console.log("Bound view-profile listener for", btn.getAttribute("data-user"));
            });
            if (debug) console.log(`loadSentRequests: DOM updated, cards = ${keys.length}, attempt = ${attempt}`);
        } catch (error) {
            console.error(`loadSentRequests error (attempt ${attempt}):`, error.message, error.stack);
            if (attempt < maxAttempts) {
                setTimeout(() => loadSentRequests(attempt + 1, maxAttempts), 100);
            } else {
                alert("Failed to load sent requests. Please refresh the page.");
            }
        }
    }

    function handleDeleteCoach() {
        const coach = this.getAttribute("data-user");
        if (debug) console.log(`handleDeleteCoach: Attempting to delete coach ${coach}`);
        try {
            if (!userRequests[currentUser] || !userRequests[currentUser][coach]) {
                throw new Error(`No request found for ${coach}`);
            }
            delete userRequests[currentUser][coach];
            if (Object.keys(userRequests[currentUser]).length === 0) {
                delete userRequests[currentUser];
            }
            if (coachRequests[coach] && coachRequests[coach][currentUser]) {
                delete coachRequests[coach][currentUser];
                if (Object.keys(coachRequests[coach]).length === 0) {
                    delete coachRequests[coach];
                }
            }
            if (acceptedStudents[coach] && acceptedStudents[coach][currentUser]) {
                delete acceptedStudents[coach][currentUser];
                if (Object.keys(acceptedStudents[coach]).length === 0) {
                    delete acceptedStudents[coach];
                }
            }
            saveData();
            loadSentRequests();
            alert(`Request to ${coach} has been deleted.`);
            if (debug) console.log(`handleDeleteCoach: Deleted coach ${coach}, DOM updated`);
        } catch (error) {
            console.error(`handleDeleteCoach error for ${coach}:`, error.message, error.stack);
            alert(`Error deleting coach ${coach}: ${error.message}. Please try again.`);
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

    function loadChatMessages(coach) {
        try {
            if (!chatMessages) {
                throw new Error("Chat messages container not found.");
            }
            chatMessages.innerHTML = "";
            const messages = chatMessagesData[currentUser]?.[coach] || [];
            if (chatMessagesData[currentUser]?.[coach]) {
                const initialUnreadCount = messages.filter(msg => msg.sender !== currentUser && !msg.read).length;
                chatMessagesData[currentUser][coach] = messages.map(msg => {
                    if (msg.sender !== currentUser && !msg.read) {
                        return { ...msg, read: true };
                    }
                    return msg;
                });
                saveData();
                if (debug) console.log(`loadChatMessages: Marked ${initialUnreadCount} messages from ${coach} as read`);
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
            loadSentRequests();
            if (debug) console.log(`loadChatMessages: Loaded ${messages.length} messages for ${coach}, badge should be updated`);
        } catch (error) {
            console.error("loadChatMessages error:", error.message, error.stack);
            alert("Failed to load chat messages. Please try again.");
        }
    }

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

    // Autocomplete functionality
    function showSuggestions(input) {
        if (!usernameSuggestions) {
            console.error("usernameSuggestions element not found.");
            return;
        }
        usernameSuggestions.innerHTML = "";
        if (!input) {
            usernameSuggestions.classList.remove("show");
            return;
        }
        const filteredUsers = allUsers.filter(user =>
            user.toLowerCase().includes(input.toLowerCase()) && user.toLowerCase() !== currentUser.toLowerCase()
        );
        if (filteredUsers.length === 0) {
            usernameSuggestions.classList.remove("show");
            return;
        }
        filteredUsers.forEach(user => {
            const suggestionItem = document.createElement("div");
            suggestionItem.className = "suggestion-item";
            suggestionItem.textContent = user;
            suggestionItem.addEventListener("click", () => {
                coachNameInput.value = user;
                usernameSuggestions.innerHTML = "";
                usernameSuggestions.classList.remove("show");
                coachNameInput.focus();
            });
            usernameSuggestions.appendChild(suggestionItem);
        });
        usernameSuggestions.classList.add("show");
        if (debug) console.log(`showSuggestions: ${filteredUsers.length} suggestions for input "${input}"`, filteredUsers);
    }

    function handleKeydown(e) {
        const suggestions = usernameSuggestions.querySelectorAll(".suggestion-item");
        if (!suggestions.length) return;
        let activeIndex = Array.from(suggestions).findIndex(item => item.classList.contains("active"));
        if (e.key === "ArrowDown") {
            e.preventDefault();
            if (activeIndex < suggestions.length - 1) {
                if (activeIndex >= 0) suggestions[activeIndex].classList.remove("active");
                activeIndex++;
                suggestions[activeIndex].classList.add("active");
                suggestions[activeIndex].scrollIntoView({ block: "nearest" });
            }
        } else if (e.key === "ArrowUp") {
            e.preventDefault();
            if (activeIndex > 0) {
                suggestions[activeIndex].classList.remove("active");
                activeIndex--;
                suggestions[activeIndex].classList.add("active");
                suggestions[activeIndex].scrollIntoView({ block: "nearest" });
            }
        } else if (e.key === "Enter" && activeIndex >= 0) {
            e.preventDefault();
            coachNameInput.value = suggestions[activeIndex].textContent;
            usernameSuggestions.innerHTML = "";
            usernameSuggestions.classList.remove("show");
        }
    }

    if (coachNameInput) {
        coachNameInput.addEventListener("input", () => showSuggestions(coachNameInput.value.trim()));
        coachNameInput.addEventListener("keydown", handleKeydown);
        // Hide suggestions when clicking outside
        document.addEventListener("click", (e) => {
            if (!coachNameInput.contains(e.target) && !usernameSuggestions.contains(e.target)) {
                usernameSuggestions.classList.remove("show");
            }
        });
    } else {
        console.error("coachNameInput not found in DOM");
    }

    function submitCoachRequestHandler() {
        let coachName = coachNameInput.value.trim();
        if (coachName) {
            try {
                if (coachName.toLowerCase() === currentUser.toLowerCase()) {
                    alert("You cannot send a request to yourself.");
                    return;
                }
                if (!allUsers.includes(coachName)) {
                    alert(`User "${coachName}" does not exist. Please select a valid user from the suggestions.`);
                    if (debug) console.log(`User check failed: ${coachName} not in allUsers`, allUsers);
                    return;
                }
                if (userRequests[currentUser] && userRequests[currentUser][coachName] === "accepted") {
                    alert(`You already have an accepted request with ${coachName}.`);
                    return;
                }
                if (!coachRequests[coachName]) {
                    coachRequests[coachName] = {};
                }
                if (!coachRequests[coachName][currentUser]) {
                    coachRequests[coachName][currentUser] = true;
                    if (!userRequests[currentUser]) {
                        userRequests[currentUser] = {};
                    }
                    userRequests[currentUser][coachName] = "pending";
                    if (debug) console.log("Before save: userRequests =", userRequests);
                    saveData();
                    setTimeout(() => {
                        coachRequests = JSON.parse(localStorage.getItem("coachRequests")) || {};
                        userRequests = JSON.parse(localStorage.getItem("userRequests")) || {};
                        if (debug) console.log("After save: userRequests =", userRequests);
                        loadSentRequests();
                        loadIncomingRequests();
                        alert(`Your request to be coached by ${coachName} has been sent!`);
                    }, 100);
                    requestCoachModal.hide();
                    coachNameInput.value = "";
                    usernameSuggestions.innerHTML = "";
                    usernameSuggestions.classList.remove("show");
                } else {
                    alert(`You have already sent a request to ${coachName}.`);
                }
            } catch (error) {
                console.error("sendRequest error:", error.message, error.stack);
                alert("An error occurred while sending the request. Please try again.");
            }
        } else {
            alert("Please provide a user name.");
        }
    }

    if (requestCoachBtn) {
        requestCoachBtn.addEventListener("click", () => {
            if (requestCoachModal) {
                requestCoachModal.show();
                coachNameInput.focus();
            } else {
                console.error("requestCoachModal not initialized");
                alert("Failed to open request modal. Please ensure Bootstrap is loaded.");
            }
        });
    } else {
        console.error("requestCoachBtn not found in DOM");
    }

    if (submitCoachRequest) {
        submitCoachRequest.addEventListener("click", submitCoachRequestHandler);
    } else {
        console.error("submitCoachRequest not found in DOM");
    }

    if (coachNameInput) {
        coachNameInput.addEventListener("keypress", (e) => {
            if (e.key === "Enter") submitCoachRequestHandler();
        });
    } else {
        console.error("coachNameInput not found in DOM");
    }

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

    try {
        loadSentRequests();
        loadIncomingRequests();
        if (debug) console.log("Initial load completed");
    } catch (error) {
        console.error("Initial load error:", error.message, error.stack);
        alert("Failed to initialize Request Coach page. Please refresh the page.");
    }
});