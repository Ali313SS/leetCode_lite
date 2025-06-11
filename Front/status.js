
document.addEventListener("DOMContentLoaded", () => {
  const submissions = [
    { user: "John", problem: "A", status: "Accepted", time: "10:02" },
    { user: "John", problem: "B", status: "Wrong Answer", time: "10:05" },
    { user: "Sara", problem: "A", status: "Accepted", time: "10:10" },
    { user: "Omar", problem: "C", status: "Time Limit", time: "10:15" }
  ];

  window.filterSubmissions = function () {
    const userFilter = document.getElementById("userFilter").value.toLowerCase();
    const statusFilter = document.getElementById("statusFilter").value;

    const tbody = document.querySelector("#submissionTable tbody");
    tbody.innerHTML = "";

    submissions.forEach(sub => {
      if (
        (userFilter === "" || sub.user.toLowerCase().includes(userFilter)) &&
        (statusFilter === "" || sub.status === statusFilter)
      ) {
        const row = document.createElement("tr");
        row.innerHTML = `
          <td>${sub.user}</td>
          <td>${sub.problem}</td>
          <td>${sub.status}</td>
          <td>${sub.time}</td>
        `;
        tbody.appendChild(row);
      }
    });
  };

  filterSubmissions(); // initial load
});
