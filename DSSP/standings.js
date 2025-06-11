
document.addEventListener("DOMContentLoaded", () => {
  const data = [
    { username: "John", score: 500, penalty: 10, solved: 5 },
    { username: "Sara", score: 400, penalty: 20, solved: 4 },
    { username: "Omar", score: 300, penalty: 25, solved: 3 },
  ];

  const table = document.querySelector("#standingsTable tbody");
  data.forEach((user, index) => {
    const row = document.createElement("tr");
    row.innerHTML = `
      <td>${index + 1}</td>
      <td>${user.username}</td>
      <td>${user.score}</td>
      <td>${user.penalty}</td>
      <td>${user.solved}</td>
    `;
    table.appendChild(row);
  });
});
