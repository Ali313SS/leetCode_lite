document.addEventListener("DOMContentLoaded", function () {
    const ojFilter = document.getElementById("ojFilter");
    const titleFilter = document.getElementById("titleFilter");
    const problemsTable = document.getElementById("problemsTable").getElementsByTagName("tr");

    
    function filterProblemsByOJ() {
        const selectedOJ = ojFilter.value.toLowerCase();
        const searchText = titleFilter.value.toLowerCase();

        for (let row of problemsTable) {
            const oj = row.getAttribute("data-oj").toLowerCase();
            const title = row.cells[2].textContent.toLowerCase();

            const matchesOJ = selectedOJ === "all" || oj === selectedOJ;
            const matchesTitle = title.includes(searchText);

            row.style.display = matchesOJ && matchesTitle ? "" : "none";
        }
    }

    
    ojFilter.addEventListener("change", filterProblemsByOJ);
    titleFilter.addEventListener("input", filterProblemsByOJ);
});


function openProblem(problemId) {
    window.location.href = `problem.html?id=${problemId}`;
}


document.addEventListener("DOMContentLoaded", function () {
    const currentPath = window.location.pathname.split("/").pop();
    const navLinks = document.querySelectorAll(".navbar a");

    navLinks.forEach(link => {
        if (link.getAttribute("href") === currentPath) {
            link.classList.add("active");
        }
    });
});


function filterProblems(filter) {
    let rows = document.querySelectorAll("#problemsTable tr");

    if (filter === "favorites") {
        let favorites = JSON.parse(localStorage.getItem("favorites")) || [];

        rows.forEach(row => {
            let problemId = row.getAttribute("onclick")?.match(/'([^']+)'/)[1]; 
            
            if (favorites.includes(problemId)) {
                row.style.display = "";  
            } else {
                row.style.display = "none";
            }
        });
    } else {
        rows.forEach(row => {
            let status = row.getAttribute("data-status"); 

            if (filter === "all" || status === filter) {
                row.style.display = "";
            } else {
                row.style.display = "none";
            }
        });
    }

    
    document.querySelectorAll(".filter-buttons button").forEach(btn => btn.classList.remove("active"));
    document.querySelector(`.filter-buttons button[onclick="filterProblems('${filter}')"]`).classList.add("active");
}
  
