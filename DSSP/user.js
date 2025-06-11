// You can add JavaScript functionality here as needed
document.addEventListener('DOMContentLoaded', () => {
    // Optional: Add event listeners or functions as necessary
});
//search
document.addEventListener("DOMContentLoaded", function () {
    const usernameSearch = document.getElementById("usernameSearch");
    const schoolSearch = document.getElementById("schoolSearch");
    const tableRows = document.querySelectorAll("#userTable tr");

    function filterTable() {
        const usernameValue = usernameSearch.value.toLowerCase();
        const schoolValue = schoolSearch.value.toLowerCase();

        tableRows.forEach(row => {
            const username = row.cells[1].textContent.toLowerCase();
            const school = row.cells[3].textContent.toLowerCase();
            
            if (username.includes(usernameValue) && school.includes(schoolValue)) {
                row.style.display = "";
            } else {
                row.style.display = "none";
            }
        });
    }

    usernameSearch.addEventListener("input", filterTable);
    schoolSearch.addEventListener("input", filterTable);
});
