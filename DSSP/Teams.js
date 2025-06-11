// Teams.js
function showMainSections() {
    const teamsSection = document.getElementById('teams-section');
    const invitationsSection = document.getElementById('invitations-section');
    const newTeamSection = document.getElementById('new-team-section');
    const teamManagementSection = document.getElementById('team-management-section');
    const teamMenuCard = document.getElementById('team-menu-card');
    const renameSection = document.getElementById('rename-section');
    const settingsSection = document.getElementById('settings-section');
    const deleteConfirmationCard = document.getElementById('delete-confirmation-card');

    teamsSection.style.display = 'block';
    invitationsSection.style.display = 'block';
    newTeamSection.style.display = 'none';
    teamManagementSection.style.display = 'none';
    teamMenuCard.style.display = 'none';
    renameSection.style.display = 'none';
    settingsSection.style.display = 'none';
    deleteConfirmationCard.style.display = 'none';
}

document.addEventListener('DOMContentLoaded', () => {
    const createTeamBtn = document.getElementById('create-team-btn');
    const teamsSection = document.getElementById('teams-section');
    const invitationsSection = document.getElementById('invitations-section');
    const newTeamSection = document.getElementById('new-team-section');
    const teamManagementSection = document.getElementById('team-management-section');
    const renameSection = document.getElementById('rename-section');
    const settingsSection = document.getElementById('settings-section');
    const newTeamNameInput = document.getElementById('new-team-name');
    const createTeamSubmitBtn = document.getElementById('create-team-submit');
    const backToTeamsFromNewBtn = document.getElementById('back-to-teams-from-new');
    const teamsList = document.getElementById('teams-list');
    const invitationsList = document.getElementById('invitations-list');
    const inviteModal = document.getElementById('invite-modal');
    const inviteTeamName = document.getElementById('invite-team-name');
    const inviteUsernameInput = document.getElementById('invite-username');
    const usernameSuggestions = document.getElementById('username-suggestions');
    const submitInviteBtn = document.getElementById('submit-invite');
    const cancelInviteBtn = document.getElementById('cancel-invite');
    const teamNameInput = document.getElementById('team-name-input');
    const saveTeamChangesBtn = document.getElementById('save-team-changes');
    const cancelRenameBtn = document.getElementById('cancel-rename');
    const teamMembersList = document.getElementById('team-members-list');
    const backToTeamsBtn = document.getElementById('back-to-teams');
    const teamMenuCard = document.getElementById('team-menu-card');
    const deleteConfirmationCard = document.getElementById('delete-confirmation-card');
    const deleteTeamNameSpan = document.getElementById('delete-team-name');
    const confirmDeleteBtn = document.getElementById('confirm-delete-btn');
    const cancelDeleteBtn = document.getElementById('cancel-delete-btn');
    const exitDeleteBtn = document.getElementById('exit-delete-btn');

    // New element for navbar username
    const teamsNavUserIdentifier = document.getElementById('teamsNavUserIdentifier');

    let currentTeamRow = null;
    let currentTeamName = '';
    let teamToDelete = null;

    const API_BASE_URL = 'https://ajudge.runasp.net/api';

    if (!createTeamBtn) {
        console.error('Create team button not found!');
        return;
    }

    function getAuthToken() {
        return localStorage.getItem('jwtToken');
    }

    function getPreferredDisplayNameFromToken() { // Added this helper for consistency
        const token = getAuthToken();
        if (!token) return 'Guest';
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            return payload.username || payload.name || payload.email || payload.nameid || payload.sub || 'Guest';
        } catch (error) {
            console.error('Error decoding JWT for display name:', error);
            return 'Guest';
        }
    }

    // Set the navbar user identifier immediately
    if (teamsNavUserIdentifier) {
        const username = getPreferredDisplayNameFromToken(); // Use the new helper
        teamsNavUserIdentifier.textContent = username;
        if (username !== "Guest") {
            teamsNavUserIdentifier.href = `profile.html?user=${encodeURIComponent(username)}`;
        } else {
            teamsNavUserIdentifier.href = "SignIn.html"; // Or your login page
        }
    }


    function renderUsernames(members) {
        if (!members || members === '-') return '-';
        const memberArray = Array.isArray(members) ? members : members.split(', ').filter(m => m);
        return memberArray.map(username =>
            `<span class="username">${username}</span>`
        ).join(', ');
    }

    // Helper to extract error message from API response
    async function getApiErrorMessage(response) {
        let errorMessage = `Error: ${response.status} ${response.statusText}`;
        try {
            const errorBody = await response.json();
            if (errorBody.title) {
                errorMessage = errorBody.title;
            }
            if (errorBody.errors) {
                const validationErrors = Object.values(errorBody.errors).flat().join('; ');
                if (validationErrors) {
                    errorMessage += ` - ${validationErrors}`;
                }
            }
        } catch (e) {
            // response body is not JSON
        }
        return errorMessage;
    }

    // --- API Calls ---

    // Load User's Teams
    async function loadUserTeams() {
        const authToken = getAuthToken();
        if (!authToken) {
            console.error('No auth token found, cannot load teams');
            teamsList.innerHTML = `
                <tr>
                    <td colspan="3">Please log in to view your teams.</td>
                </tr>
            `;
            return;
        }

        try {
            const response = await fetch(`${API_BASE_URL}/Teams/MyTeams`, {
                headers: {
                    'Authorization': `Bearer ${authToken}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const teams = await response.json();
                teamsList.innerHTML = ''; // Clear existing placeholders
                if (!teams || teams.length === 0) {
                    teamsList.innerHTML = `
                        <tr>
                            <td colspan="3">No teams found. Create one to get started!</td>
                        </tr>
                    `;
                } else {
                    // Ensure the header row is always present
                    teamsList.innerHTML = `
                        <tr>
                            <th>Name</th>
                            <th>Current members</th>
                            <th>Inactive members</th>
                        </tr>
                    `;
                    teams.forEach(team => {
                        const row = document.createElement('tr');
                        row.setAttribute('data-team-name', team.name);
                        row.setAttribute('data-team-id', team.teamId); // Set teamId from API response
                        const currentMembers = team.members.map(m => m.username);
                        // Assuming 'inactiveMembers' might be returned, otherwise just '-'
                        const inactiveMembers = team.inactiveMembers ? team.inactiveMembers.map(m => m.username) : [];

                        row.innerHTML = `
                            <td><span class="team-name">${team.name}</span></td>
                            <td>${renderUsernames(currentMembers)}</td>
                            <td>${renderUsernames(inactiveMembers.length > 0 ? inactiveMembers : '-')}</td>
                        `;
                        teamsList.appendChild(row);
                    });
                }
            } else {
                const errorMessage = await getApiErrorMessage(response);
                console.error('Failed to load teams:', errorMessage);
                teamsList.innerHTML = `
                    <tr>
                        <td colspan="3">Error loading teams: ${errorMessage}</td>
                    </tr>
                `;
            }
        } catch (error) {
            console.error('Error loading teams:', error);
            teamsList.innerHTML = `
                <tr>
                    <td colspan="3">Error loading teams: ${error.message}</td>
                </tr>
            `;
        }
    }

    // Load User's Invitations
    async function loadMyInvitations() {
        const authToken = getAuthToken();
        if (!authToken) {
            console.error('No auth token found, cannot load invitations');
            invitationsList.innerHTML = `
                <tr>
                    <td colspan="3">Please log in to view your invitations.</td>
                </tr>
            `;
            return;
        }

        try {
            const response = await fetch(`${API_BASE_URL}/Teams/GetMyInvitations`, {
                headers: {
                    'Authorization': `Bearer ${authToken}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const invitations = await response.json();
                invitationsList.innerHTML = ''; // Clear existing placeholders
                if (!invitations || invitations.length === 0) {
                    invitationsList.innerHTML = `
                        <tr>
                            <td colspan="3">No outstanding invitations.</td>
                        </tr>
                    `;
                } else {
                    // Ensure the header row is always present
                    invitationsList.innerHTML = `
                        <tr>
                            <th>Name</th>
                            <th>Current members</th>
                            <th>Action</th>
                        </tr>
                    `;
                    invitations.forEach(invitation => {
                        const row = document.createElement('tr');
                        row.setAttribute('data-team-name', invitation.name);
                        row.setAttribute('data-team-id', invitation.teamId); // Store teamId for actions
                        const currentMembers = invitation.members.map(m => m.username);

                        row.innerHTML = `
                            <td>${invitation.name}</td>
                            <td>${renderUsernames(currentMembers)}</td>
                            <td>
                                <div class="action-buttons">
                                    <button class="action-button accept-button">
                                        <svg class="action-svgIcon accept-svgIcon" viewBox="0 0 512 512">
                                            <path d="M470.6 105.4c12.5 12.5 12.5 32.8 0 45.3l-256 256c-12.5 12.5-32.8 12.5-45.3 0l-128-128c-12.5-12.5-12.5-32.8 0-45.3s32.8-12.5 45.3 0L192 338.7 425.4 105.4c12.5-12.5 32.8-12.5 45.3 0z"></path>
                                        </svg>
                                    </button>
                                    <button class="action-button decline-button">
                                        <svg class="action-svgIcon decline-svgIcon" viewBox="0 0 448 512">
                                            <path d="M135.2 17.7L128 32H32C14.3 32 0 46.3 0 64S14.3 96 32 96H416c17.7 0 32-14.3 32-32s-14.3-32-32-32H320l-7.2-14.3C307.4 6.8 296.3 0 284.2 0H163.8c-12.1 0-23.2 6.8-28.6 17.7zM416 128H32L53.2 467c1.6 25.3 22.6 45 47.9 45H346.9c25.3 0 46.3-19.7 47.9-45L416 128z"></path>
                                        </svg>
                                    </button>
                                </div>
                            </td>
                        `;
                        invitationsList.appendChild(row);
                    });
                }
            } else {
                const errorMessage = await getApiErrorMessage(response);
                console.error('Failed to load invitations:', errorMessage);
                invitationsList.innerHTML = `
                    <tr>
                        <td colspan="3">Error loading invitations: ${errorMessage}</td>
                    </tr>
                `;
            }
        } catch (error) {
            console.error('Error loading invitations:', error);
            invitationsList.innerHTML = `
                <tr>
                    <td colspan="3">Error loading invitations: ${error.message}</td>
                </tr>
            `;
        }
    }

    // Initial load
    showMainSections();
    loadUserTeams();
    loadMyInvitations();

    // --- Event Listeners (updated with API calls) ---

    document.addEventListener('click', (e) => {
        if (!teamMenuCard.contains(e.target) && !e.target.classList.contains('team-name')) {
            teamMenuCard.style.display = 'none';
        }
    });

    document.addEventListener('click', (e) => {
        if (e.target.classList.contains('username')) {
            const username = e.target.textContent;
            window.location.href = `profile.html?user=${encodeURIComponent(username)}`;
        }
    });

    createTeamBtn.addEventListener('click', () => {
        console.log('Create team button clicked');
        teamsSection.style.display = 'none';
        invitationsSection.style.display = 'none';
        newTeamSection.style.display = 'block';
        teamManagementSection.style.display = 'none';
        teamMenuCard.style.display = 'none';
        newTeamNameInput.value = '';
    });

    createTeamSubmitBtn.addEventListener('click', async () => {
        const newTeamName = newTeamNameInput.value.trim();
        if (newTeamName) {
            const authToken = getAuthToken();
            if (!authToken) {
                alert('You must be logged in to create a team. Please log in.');
                return;
            }

            try {
                const response = await fetch(`${API_BASE_URL}/Teams/CreateTeam`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${authToken}`
                    },
                    body: JSON.stringify({ name: newTeamName })
                });

                if (response.ok) {
                    const data = await response.json();
                    console.log('Team created successfully:', data);

                    // Reload teams after creation to ensure UI is up-to-date
                    await loadUserTeams();

                    alert('Team created: ' + data.name);
                    showMainSections();
                    newTeamNameInput.value = '';
                } else {
                    const errorMessage = await getApiErrorMessage(response);
                    alert(errorMessage);
                    console.error('Failed to create team:', errorMessage);
                }
            } catch (error) {
                alert('An error occurred while creating the team: ' + error.message);
                console.error('Error creating team:', error);
            }
        } else {
            alert('Please enter a team name');
        }
    });

    backToTeamsFromNewBtn.addEventListener('click', () => {
        showMainSections();
        newTeamNameInput.value = '';
    });

    teamsList.addEventListener('click', (e) => {
        if (e.target.classList.contains('team-name')) {
            currentTeamRow = e.target.closest('tr');
            currentTeamName = e.target.textContent;

            const rect = e.target.getBoundingClientRect();
            teamMenuCard.style.top = `${rect.bottom + window.scrollY}px`;
            teamMenuCard.style.left = `${rect.left + window.scrollX}px`;
            teamMenuCard.style.display = 'block';
        }
    });

    teamMenuCard.addEventListener('click', (e) => {
        const actionElement = e.target.closest('.element');
        if (!actionElement) return;

        const action = actionElement.getAttribute('data-action');
        teamMenuCard.style.display = 'none';

        if (action === 'rename') {
            teamNameInput.value = currentTeamName;
            teamsSection.style.display = 'none';
            invitationsSection.style.display = 'none';
            newTeamSection.style.display = 'none';
            teamManagementSection.style.display = 'block';
            renameSection.style.display = 'block';
            settingsSection.style.display = 'none';
        } else if (action === 'add-member') {
            inviteTeamName.textContent = currentTeamName;
            inviteModal.style.display = 'flex';
            inviteUsernameInput.value = '';
            usernameSuggestions.style.display = 'none';
        } else if (action === 'settings') {
            // When opening settings, reload members from the currentTeamRow's data
            const currentMembersText = currentTeamRow.cells[1].textContent;
            const inactiveMembersText = currentTeamRow.cells[2].textContent;

            const currentMembers = currentMembersText === '-' ? [] : currentMembersText.split(', ').map(s => s.trim());
            const inactiveMembers = inactiveMembersText === '-' ? [] : inactiveMembersText.split(', ').map(s => s.trim());

            teamMembersList.innerHTML = `
                <tr>
                    <th>User</th>
                    <th>Status</th>
                    <th>Action</th>
                </tr>
            `; // Clear and add header
            currentMembers.forEach(member => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td><span class="username">${member}</span></td>
                    <td>Active</td>
                    <td>
                        <div class="action-buttons">
                            <button class="action-button remove-button">
                                <svg class="action-svgIcon remove-svgIcon" viewBox="0 0 448 512">
                                    <path d="M135.2 17.7L128 32H32C14.3 32 0 46.3 0 64S14.3 96 32 96H416c17.7 0 32-14.3 32-32s-14.3-32-32-32H320l-7.2-14.3C307.4 6.8 296.3 0 284.2 0H163.8c-12.1 0-23.2 6.8-28.6 17.7zM416 128H32L53.2 467c1.6 25.3 22.6 45 47.9 45H346.9c25.3 0 46.3-19.7 47.9-45L416 128z"></path>
                                </svg>
                            </button>
                        </div>
                    </td>
                `;
                teamMembersList.appendChild(row);
            });
            inactiveMembers.forEach(member => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td><span class="username">${member}</span></td>
                    <td>Inactive</td>
                    <td>
                        <div class="action-buttons">
                            <button class="action-button remove-button">
                                <svg class="action-svgIcon remove-svgIcon" viewBox="0 0 448 512">
                                    <path d="M135.2 17.7L128 32H32C14.3 32 0 46.3 0 64S14.3 96 32 96H416c17.7 0 32-14.3 32-32s-14.3-32-32-32H320l-7.2-14.3C307.4 6.8 296.3 0 284.2 0H163.8c-12.1 0-23.2 6.8-28.6 17.7zM416 128H32L53.2 467c1.6 25.3 22.6 45 47.9 45H346.9c25.3 0 46.3-19.7 47.9-45L416 128z"></path>
                                </svg>
                            </button>
                        </div>
                    </td>
                `;
                teamMembersList.appendChild(row);
            });

            teamsSection.style.display = 'none';
            invitationsSection.style.display = 'none';
            newTeamSection.style.display = 'none';
            teamManagementSection.style.display = 'block';
            renameSection.style.display = 'none';
            settingsSection.style.display = 'block';
        } else if (action === 'delete') {
            teamToDelete = currentTeamRow;
            deleteTeamNameSpan.textContent = currentTeamName;
            deleteConfirmationCard.style.display = 'flex';
        }
    });

    confirmDeleteBtn.addEventListener('click', async () => {
        if (teamToDelete) {
            const teamId = teamToDelete.getAttribute('data-team-id');
            const teamName = teamToDelete.getAttribute('data-team-name');
            const authToken = getAuthToken();

            if (!teamId) {
                alert('Team ID not found. Cannot delete team.');
                return;
            }
            if (!authToken) {
                alert('You must be logged in to delete a team. Please log in.');
                return;
            }

            try {
                const response = await fetch(`${API_BASE_URL}/Teams/${teamId}/leave`, {
                    method: 'POST',
                    headers: {
                        'Authorization': `Bearer ${authToken}`
                    }
                });

                if (response.ok) {
                    alert(`Team "${teamName}" deleted successfully!`);
                    await loadUserTeams();
                    await loadMyInvitations();
                    showMainSections();
                } else {
                    const errorMessage = await getApiErrorMessage(response);
                    alert(`Failed to delete team: ${errorMessage}`);
                    console.error('Failed to delete team:', errorMessage);
                }
            } catch (error) {
                alert('An error occurred while deleting the team: ' + error.message);
                console.error('Error deleting team:', error);
            } finally {
                teamToDelete = null;
                deleteConfirmationCard.style.display = 'none';
            }
        }
    });

    cancelDeleteBtn.addEventListener('click', () => {
        deleteConfirmationCard.style.display = 'none';
        teamToDelete = null;
    });

    exitDeleteBtn.addEventListener('click', () => {
        deleteConfirmationCard.style.display = 'none';
        teamToDelete = null;
    });

    saveTeamChangesBtn.addEventListener('click', async () => {
        const newName = teamNameInput.value.trim();
        const teamId = currentTeamRow.getAttribute('data-team-id');
        const authToken = getAuthToken();

        if (!newName) {
            alert('Please enter a team name');
            return;
        }
        if (!teamId) {
            alert('Team ID not found. Cannot rename team.');
            return;
        }
        if (!authToken) {
            alert('You must be logged in to rename a team. Please log in.');
            return;
        }

        try {
            // Placeholder/simulated update if API for renaming is not ready
            currentTeamRow.setAttribute('data-team-name', newName);
            currentTeamRow.cells[0].innerHTML = `<span class="team-name">${newName}</span>`;
            alert('Team name updated to: ' + newName);
            showMainSections();
        } catch (error) {
            alert('An error occurred while renaming the team: ' + error.message);
            console.error('Error renaming team:', error);
        }
    });

    cancelRenameBtn.addEventListener('click', () => {
        showMainSections();
    });

    teamMembersList.addEventListener('click', async (e) => {
        if (e.target.closest('.remove-button')) {
            const row = e.target.closest('tr');
            const memberUsername = row.cells[0].textContent;
            const status = row.cells[1].textContent;
            const teamId = currentTeamRow.getAttribute('data-team-id');
            const teamName = currentTeamRow.getAttribute('data-team-name');
            const authToken = getAuthToken();

            if (!teamId) {
                alert('Team ID not found. Cannot remove member.');
                return;
            }
            if (!authToken) {
                alert('You must be logged in to remove a member. Please log in.');
                return;
            }

            if (status === 'Active' && currentTeamRow.cells[1].textContent.split(', ').filter(m => m).length === 1) {
                alert('A team must have at least one active member');
                return;
            }

            if (confirm(`Are you sure you want to remove ${memberUsername} from ${teamName}?`)) {
                try {
                    alert('Removing member feature not implemented in API yet.'); // Placeholder alert
                    row.remove(); // Simulate UI update
                    alert(`${memberUsername} has been removed from the team (simulated).`);
                } catch (error) {
                    alert('An error occurred while removing the member: ' + error.message);
                    console.error('Error removing member:', error);
                }
            }
        }
    });

    teamsList.addEventListener('click', (e) => {
        if (e.target.classList.contains('invite-btn')) {
            currentTeamRow = e.target.closest('tr');
            const teamName = currentTeamRow.getAttribute('data-team-name');
            inviteTeamName.textContent = teamName;
            inviteModal.style.display = 'flex';
        }
    });

    submitInviteBtn.addEventListener('click', async () => {
        const username = inviteUsernameInput.value.trim();
        const teamName = inviteTeamName.textContent;
        const teamId = currentTeamRow.getAttribute('data-team-id');

        if (!username) {
            alert('Please enter a username');
            return;
        }

        if (!teamId) {
            alert('Team ID not found. Cannot send invitation. Please try reloading the page.');
            console.error('Team ID is missing for currentTeamRow during invite:', currentTeamRow);
            return;
        }

        const authToken = getAuthToken();
        if (!authToken) {
            alert('You must be logged in to invite a user. Please log in.');
            return;
        }

        try {
            const response = await fetch(`${API_BASE_URL}/Teams/InviteUserToTeam?teamId=${teamId}&username=${encodeURIComponent(username)}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${authToken}`
                }
            });

            if (response.ok) {
                alert(`User "${username}" invited to team "${teamName}"`);
                await loadUserTeams();
                await loadMyInvitations();
                inviteModal.style.display = 'none';
                inviteUsernameInput.value = '';
                currentTeamRow = null;
            } else {
                const errorMessage = await getApiErrorMessage(response);
                alert(errorMessage);
                console.error('Failed to send invitation:', errorMessage);
            }
        } catch (error) {
            alert('An error occurred while inviting the user: ' + error.message);
            console.error('Error inviting user:', error);
        }
    });

    cancelInviteBtn.addEventListener('click', () => {
        inviteModal.style.display = 'none';
        inviteUsernameInput.value = '';
        usernameSuggestions.style.display = 'none';
        currentTeamRow = null;
    });

    invitationsList.addEventListener('click', async (e) => {
        const acceptButton = e.target.closest('.action-button.accept-button');
        const declineButton = e.target.closest('.action-button.decline-button');
        if (!acceptButton && !declineButton) return;

        const row = e.target.closest('tr');
        if (!row) {
            console.error('Invitation row not found');
            return;
        }

        const teamName = row.getAttribute('data-team-name');
        const teamId = row.getAttribute('data-team-id');
        const authToken = getAuthToken();

        if (!teamId) {
            alert('Invitation Team ID not found. Cannot process action.');
            return;
        }
        if (!authToken) {
            alert('You must be logged in to respond to invitations. Please log in.');
            return;
        }

        let apiEndpoint = '';
        let successMessage = '';
        let confirmMessage = '';

        if (acceptButton) {
            apiEndpoint = `${API_BASE_URL}/Teams/accept-invitation?teamId=${teamId}`;
            successMessage = `You have joined the team "${teamName}"`;
            confirmMessage = `Are you sure you want to accept the invitation to "${teamName}"?`;
        } else if (declineButton) {
            apiEndpoint = `${API_BASE_URL}/Teams/reject-invitation?teamId=${teamId}`;
            successMessage = `Invitation to "${teamName}" declined`;
            confirmMessage = `Are you sure you want to decline the invitation to "${teamName}"?`;
        }

        if (confirm(confirmMessage)) {
            try {
                const response = await fetch(apiEndpoint, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${authToken}`
                    }
                });

                if (response.ok) {
                    alert(successMessage);
                    await loadUserTeams();
                    await loadMyInvitations();
                } else {
                    const errorMessage = await getApiErrorMessage(response);
                    alert(`Failed to process invitation: ${errorMessage}`);
                    console.error('Failed to process invitation:', errorMessage);
                }
            } catch (error) {
                alert('An error occurred while processing the invitation: ' + error.message);
                console.error('Error processing invitation:', error);
            }
        }
    });

    backToTeamsBtn.addEventListener('click', showMainSections);
});