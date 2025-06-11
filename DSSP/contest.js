document.addEventListener("DOMContentLoaded", function () {
    const titleFilter = document.querySelector("th:nth-child(2) input");
    const ownerFilter = document.querySelector("th:nth-child(5) input");
    const contestRows = document.querySelectorAll("tbody tr");
    const filterButtons = document.querySelectorAll(".filter-btn");

    function filterContests() {
        const titleText = titleFilter.value.trim().toLowerCase();
        const ownerText = ownerFilter.value.trim().toLowerCase();
        const activeFilter = document.querySelector(".filter-btn.active").dataset.filter;

        contestRows.forEach(row => {
            const title = row.cells[1].textContent.trim().toLowerCase();
            const ownerElement = row.cells[4].querySelector("a"); // الحصول على عنصر المالك
            const owner = ownerElement ? ownerElement.textContent.trim().toLowerCase() : ""; // تجنب الأخطاء
            const status = row.classList.contains("scheduled") ? "scheduled"
                          : row.classList.contains("running") ? "running"
                          : "ended";

            const matchesTitle = title.includes(titleText);
            const matchesOwner = owner.includes(ownerText);
            const matchesFilter = activeFilter === "all" || status === activeFilter;

            row.style.display = matchesTitle && matchesOwner && matchesFilter ? "" : "none";
        });
    }

    
    titleFilter.addEventListener("input", filterContests);
    ownerFilter.addEventListener("input", filterContests);

    
    filterButtons.forEach(button => {
        button.addEventListener("click", function () {
            filterButtons.forEach(btn => btn.classList.remove("active"));
            this.classList.add("active");
            filterContests();
        });
    });

    
    document.querySelector(".filter-btn[data-filter='all']").classList.add("active");

   
    contestRows.forEach(row => {
        row.addEventListener("click", function () {
            const contestId = row.cells[0].textContent.trim();
            window.location.href = `contest-info.html?id=${contestId}`;
        });
    });
});