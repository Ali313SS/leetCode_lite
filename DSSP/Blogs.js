// Initialize blog posts from localStorage or empty array
let blogPosts = [];
try {
    const storedPosts = localStorage.getItem('blogPosts');
    if (storedPosts) {
        blogPosts = JSON.parse(storedPosts).map(post => ({
            ...post,
            showComments: post.showComments !== undefined ? post.showComments : false, // Ensure backward compatibility
            comments: post.comments.map(comment => ({ // Ensure comments have like/dislike properties
                ...comment,
                likes: comment.likes !== undefined ? comment.likes : 0,
                dislikes: comment.dislikes !== undefined ? comment.dislikes : 0,
                liked: comment.liked !== undefined ? comment.liked : false,
                disliked: comment.disliked !== undefined ? comment.disliked : false
            }))
        }));
    }
} catch (e) {
    console.error('Error reading from localStorage:', e);
}

const currentUser = "Basant"; // Placeholder; replace with actual user authentication logic

document.addEventListener('DOMContentLoaded', () => {
    renderPosts();
});

function toggleBlogForm() {
    const blogForm = document.getElementById('blogForm');
    const createBlogSection = document.querySelector('.create-blog-section');
    const blogContainer = document.getElementById('blogContainer');

    if (blogForm.classList.contains('active')) {
        blogForm.classList.remove('active');
        createBlogSection.style.display = 'block';
        blogContainer.style.display = 'block';
    } else {
        blogForm.classList.add('active');
        createBlogSection.style.display = 'none';
        blogContainer.style.display = 'none';
    }
}

function addNewPost() {
    const title = document.getElementById('postTitle').value.trim();
    const content = document.getElementById('postContent').value.trim();

    if (title && content) {
        const newPost = {
            title: title,
            content: content,
            username: currentUser,
            likes: 0,
            dislikes: 0,
            liked: false,
            disliked: false,
            comments: [],
            createdAt: new Date().toLocaleString(),
            isEditing: false,
            showComments: false // Initialize comments section as hidden
        };

        blogPosts.unshift(newPost);
        updateStorage();

        document.getElementById('postTitle').value = '';
        document.getElementById('postContent').value = '';
        toggleBlogForm();
        renderPosts();
    } else {
        alert('Please fill in all fields: Title and Content.');
    }
}

function renderPosts() {
    const container = document.getElementById('blogContainer');
    container.innerHTML = '';

    blogPosts.forEach((post, index) => {
        const postElement = document.createElement('div');
        postElement.className = 'blog-post';
        postElement.innerHTML = `
            ${post.isEditing ? `
                <div class="edit-form active">
                    <input type="text" id="editPostTitle-${index}" value="${post.title}">
                    <textarea id="editPostContent-${index}">${post.content}</textarea>
                    <button onclick="savePost(${index})">Save</button>
                    <a href="#" onclick="cancelEditPost(${index})">Cancel</a>
                </div>
            ` : `
                <h3>${post.title}</h3>
                <div class="post-meta">
                    Posted by: <a href="profile.html?username=${encodeURIComponent(post.username)}" class="username-link">${post.username}</a> on ${post.createdAt}
                    ${post.username === currentUser ? `
                        <button class="Btn" onclick="editPost(${index})">
                            Edit
                            <svg viewBox="0 0 512 512" class="svg">
                                <path d="M410.3 231l11.3-11.3-33.9-33.9-62.1-62.1L291.7 89.8l-11.3 11.3-22.6 22.6L58.6 322.9c-10.4 10.4-18 23.3-22.2 37.4L1 480.7c-2.5 8.4-.2 17.5 6.1 23.7s15.3 8.5 23.7 6.1l120.3-35.4c14.1-4.2 27-11.8 37.4-22.2L387.7 253.7 410.3 231zM160 399.4l-9.1 22.7c-4 3.1-8.5 5.4-13.3 6.9L59.4 452l23-78.1c1.4-4.9 3.8-9.4 6.9-13.3l22.7-9.1v32c0 8.8 7.2 16 16 16h32zM362.7 18.7L348.3 33.2 325.7 55.8 314.3 67.1l33.9 33.9 62.1 62.1 33.9 33.9 11.3-11.3 22.6-22.6 14.5-14.5c25-25 25-65.5 0-90.5L453.3 18.7c-25-25-65.5-25-90.5 0zm-47.4 168l-144 144c-6.2 6.2-16.4 6.2-22.6 0s-6.2-16.4 0-22.6l144-144c6.2-6.2 16.4-6.2 22.6 0s6.2 16.4 0 22.6z"></path>
                            </svg>
                        </button>
                        <button aria-label="Delete post" class="delete-button" onclick="deletePost(${index})">
                            <svg class="trash-svg" viewBox="0 -10 64 74" xmlns="http://www.w3.org/2000/svg">
                                <g id="trash-can">
                                    <rect x="16" y="24" width="32" height="30" rx="3" ry="3" fill="#e74c3c"></rect>
                                    <g transform-origin="12 18" id="lid-group">
                                        <rect x="12" y="12" width="40" height="6" rx="2" ry="2" fill="#c0392b"></rect>
                                        <rect x="26" y="8" width="12" height="4" rx="2" ry="2" fill="#c0392b"></rect>
                                    </g>
                                </g>
                            </svg>
                        </button>
                    ` : ''}
                </div>
                <p>${post.content}</p>
            `}
            <div class="reactions">
                <button class="like-button" onclick="toggleLike(${index})">
                    <svg class="like-icon" viewBox="0 0 24 24">
                        <path d="M12 21.35l-1.45-1.32C5.4 15.36 2 12.28 2 8.5 2 5.42 4.42 3 7.5 3c1.74 0 3.41.81 4.5 2.09C13.09 3.81 14.76 3 16.5 3 19.58 3 22 5.42 22 8.5c0 3.78-3.4 6.86-8.55 11.54L12 21.35z"/>
                    </svg>
                    <span class="like-text">Like</span>
                    <span class="like-count">${post.likes}</span>
                </button>
                <button class="dislike-button" onclick="toggleDislike(${index})">
                    <svg class="dislike-icon" viewBox="0 0 24 24">
                        <path d="M12 2.65l1.45 1.32C18.6 8.64 22 11.72 22 15.5c0 3.08-2.42 5.5-5.5 5.5-1.74 0-3.41-.81-4.5-2.09C10.91 20.69 9.24 21.5 7.5 21.5 4.42 21.5 2 19.08 2 16c0-3.78 3.4-6.86 8.55-11.54L12 2.65z"/>
                    </svg>
                    <span class="dislike-text">Dislike</span>
                    <span class="dislike-count">${post.dislikes}</span>
                </button>
                <button class="comment-button" onclick="toggleComments(${index})">
                    <svg class="comment-icon" viewBox="0 0 24 24">
                        <path d="M21.99 4c0-1.1-.89-2-1.99-2H4C2.9 2 2 2.9 2 4v12c0 1.1.9 2 2 2h14l4 4-.01-18zM18 14H6v-2h12v2zm0-3H6V9h12v2zm0-3H6V6h12v2z"/>
                    </svg>
                    <span class="comment-text">Comment</span>
                    <span class="comment-count">${post.comments.length}</span>
                </button>
            </div>
            <div class="comments-section${post.showComments ? ' active' : ''}">
                <h4>Comments (${post.comments.length})</h4>
                ${post.comments.map((comment, commentIndex) => `
                    <div class="comment">
                        ${comment.isEditing ? `
                            <div class="edit-form active">
                                <textarea id="editCommentText-${index}-${commentIndex}">${comment.text}</textarea>
                                <button onclick="saveComment(${index}, ${commentIndex})">Save</button>
                                <a href="#" onclick="cancelEditComment(${index}, ${commentIndex})">Cancel</a>
                            </div>
                        ` : `
                            <div>
                                <p>${comment.text}</p>
                                <small class="comment-time">Posted by: ${comment.username} on ${comment.date}</small>
                                <div class="reactions">
                                    <button class="like-button" onclick="toggleLikeComment(${index}, ${commentIndex})">
                                        <svg class="like-icon" viewBox="0 0 24 24">
                                            <path d="M12 21.35l-1.45-1.32C5.4 15.36 2 12.28 2 8.5 2 5.42 4.42 3 7.5 3c1.74 0 3.41.81 4.5 2.09C13.09 3.81 14.76 3 16.5 3 19.58 3 22 5.42 22 8.5c0 3.78-3.4 6.86-8.55 11.54L12 21.35z"/>
                                        </svg>
                                        <span class="like-count">${comment.likes}</span>
                                    </button>
                                    <button class="dislike-button" onclick="toggleDislikeComment(${index}, ${commentIndex})">
                                        <svg class="dislike-icon" viewBox="0 0 24 24">
                                            <path d="M12 2.65l1.45 1.32C18.6 8.64 22 11.72 22 15.5c0 3.08-2.42 5.5-5.5 5.5-1.74 0-3.41-.81-4.5-2.09C10.91 20.69 9.24 21.5 7.5 21.5 4.42 21.5 2 19.08 2 16c0-3.78 3.4-6.86 8.55-11.54L12 2.65z"/>
                                        </svg>
                                        <span class="dislike-count">${comment.dislikes}</span>
                                    </button>
                                </div>
                            </div>
                            ${comment.username === currentUser ? `
                                <button class="Btn" onclick="editComment(${index}, ${commentIndex})">
                                    Edit
                                    <svg viewBox="0 0 512 512" class="svg">
                                        <path d="M410.3 231l11.3-11.3-33.9-33.9-62.1-62.1L291.7 89.8l-11.3 11.3-22.6 22.6L58.6 322.9c-10.4 10.4-18 23.3-22.2 37.4L1 480.7c-2.5 8.4-.2 17.5 6.1 23.7s15.3 8.5 23.7 6.1l120.3-35.4c14.1-4.2 27-11.8 37.4-22.2L387.7 253.7 410.3 231zM160 399.4l-9.1 22.7c-4 3.1-8.5 5.4-13.3 6.9L59.4 452l23-78.1c1.4-4.9 3.8-9.4 6.9-13.3l22.7-9.1v32c0 8.8 7.2 16 16 16h32zM362.7 18.7L348.3 33.2 325.7 55.8 314.3 67.1l33.9 33.9 62.1 62.1 33.9 33.9 11.3-11.3 22.6-22.6 14.5-14.5c25-25 25-65.5 0-90.5L453.3 18.7c-25-25-65.5-25-90.5 0zm-47.4 168l-144 144c-6.2 6.2-16.4 6.2-22.6 0s-6.2-16.4 0-22.6l144-144c6.2-6.2 16.4-6.2 22.6 0s6.2 16.4 0 22.6z"></path>
                                    </svg>
                                </button>
                                <button aria-label="Delete comment" class="delete-button" onclick="deleteComment(${index}, ${commentIndex})">
                                    <svg class="trash-svg" viewBox="0 -10 64 74" xmlns="http://www.w3.org/2000/svg">
                                        <g id="trash-can">
                                            <rect x="16" y="24" width="32" height="30" rx="3" ry="3" fill="#e74c3c"></rect>
                                            <g transform-origin="12 18" id="lid-group">
                                                <rect x="12" y="12" width="40" height="6" rx="2" ry="2" fill="#c0392b"></rect>
                                                <rect x="26" y="8" width="12" height="4" rx="2" ry="2" fill="#c0392b"></rect>
                                            </g>
                                        </g>
                                    </svg>
                                </button>
                            ` : ''}
                        `}
                    </div>
                `).join('')}
                <input type="text" id="comment-${index}" placeholder="Add a comment">
                <button onclick="addComment(${index})">Add Comment</button>
            </div>
        `;
        container.appendChild(postElement);
    });
}

function toggleLike(index) {
    const post = blogPosts[index];
    if (!post.liked) {
        post.likes++;
        post.liked = true;
        if (post.disliked) {
            post.dislikes--;
            post.disliked = false;
        }
    }
    updateStorage();
    renderPosts();
}

function toggleDislike(index) {
    const post = blogPosts[index];
    if (!post.disliked) {
        post.dislikes++;
        post.disliked = true;
        if (post.liked) {
            post.likes--;
            post.liked = false;
        }
    }
    updateStorage();
    renderPosts();
}

function toggleComments(index) {
    blogPosts[index].showComments = !blogPosts[index].showComments;
    updateStorage();
    renderPosts();
    if (blogPosts[index].showComments) {
        const commentsSection = document.querySelector(`#blogContainer .blog-post:nth-child(${index + 1}) .comments-section`);
        if (commentsSection) {
            commentsSection.scrollIntoView({ behavior: 'smooth' });
        }
    }
}

function addComment(index) {
    const commentInput = document.getElementById(`comment-${index}`);
    const commentText = commentInput.value.trim();
    if (commentText) {
        blogPosts[index].comments.push({
            text: commentText,
            date: new Date().toLocaleString(),
            username: currentUser,
            isEditing: false,
            likes: 0, // Initialize likes for comment
            dislikes: 0, // Initialize dislikes for comment
            liked: false,
            disliked: false
        });
        commentInput.value = '';
        blogPosts[index].showComments = true; // Show comments section after adding a comment
        updateStorage();
        renderPosts();
    }
}

function toggleLikeComment(postIndex, commentIndex) {
    const comment = blogPosts[postIndex].comments[commentIndex];
    if (!comment.liked) {
        comment.likes++;
        comment.liked = true;
        if (comment.disliked) {
            comment.dislikes--;
            comment.disliked = false;
        }
    }
    updateStorage();
    renderPosts();
}

function toggleDislikeComment(postIndex, commentIndex) {
    const comment = blogPosts[postIndex].comments[commentIndex];
    if (!comment.disliked) {
        comment.dislikes++;
        comment.disliked = true;
        if (comment.liked) {
            comment.likes--;
            comment.liked = false;
        }
    }
    updateStorage();
    renderPosts();
}

function deletePost(index) {
    if (confirm('Are you sure you want to delete this post?')) {
        blogPosts.splice(index, 1);
        updateStorage();
        renderPosts();
    }
}

function deleteComment(postIndex, commentIndex) {
    if (confirm('Are you sure you want to delete this comment?')) {
        blogPosts[postIndex].comments.splice(commentIndex, 1);
        updateStorage();
        renderPosts();
    }
}

function editPost(index) {
    blogPosts[index].isEditing = true;
    updateStorage();
    renderPosts();
}

function savePost(index) {
    const title = document.getElementById(`editPostTitle-${index}`).value.trim();
    const content = document.getElementById(`editPostContent-${index}`).value.trim();
    if (title && content) {
        blogPosts[index].title = title;
        blogPosts[index].content = content;
        blogPosts[index].isEditing = false;
        blogPosts[index].createdAt = new Date().toLocaleString(); // Update timestamp
        updateStorage();
        renderPosts();
    } else {
        alert('Please fill in all fields: Title and Content.');
    }
}

function cancelEditPost(index) {
    blogPosts[index].isEditing = false;
    updateStorage();
    renderPosts();
}

function editComment(postIndex, commentIndex) {
    blogPosts[postIndex].comments[commentIndex].isEditing = true;
    updateStorage();
    renderPosts();
}

function saveComment(postIndex, commentIndex) {
    const text = document.getElementById(`editCommentText-${postIndex}-${commentIndex}`).value.trim();
    if (text) {
        blogPosts[postIndex].comments[commentIndex].text = text;
        blogPosts[postIndex].comments[commentIndex].isEditing = false;
        blogPosts[postIndex].comments[commentIndex].date = new Date().toLocaleString(); // Update timestamp
        updateStorage();
        renderPosts();
    } else {
        alert('Comment cannot be empty.');
    }
}

function cancelEditComment(postIndex, commentIndex) {
    blogPosts[postIndex].comments[commentIndex].isEditing = false;
    updateStorage();
    renderPosts();
}

function updateStorage() {
    try {
        localStorage.setItem('blogPosts', JSON.stringify(blogPosts));
    } catch (e) {
        console.error('Error saving to localStorage:', e);
        alert('Failed to save data. LocalStorage might be full or disabled.');
    }
}