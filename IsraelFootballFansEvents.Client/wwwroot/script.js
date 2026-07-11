const API_BASE = "https://localhost:7193/api";

function clearElement(elementId) {
    document.getElementById(elementId).innerHTML = "";
}

function showError(elementId, message) {
    document.getElementById(elementId).innerHTML =
        `<p class="error">${message}</p>`;
}

function formatDateTimeDisplay(dateString) {
    if (!dateString) {
        return "";
    }

    const cleanDate = dateString.replace("T", " ");
    const datePart = cleanDate.split(" ")[0];
    const timePart = cleanDate.split(" ")[1];

    const datePieces = datePart.split("-");
    const year = datePieces[0];
    const month = datePieces[1];
    const day = datePieces[2];

    let hours = "00";
    let minutes = "00";

    if (timePart) {
        const timePieces = timePart.split(":");
        hours = timePieces[0];
        minutes = timePieces[1];
    }

    return `${day}/${month}/${year} ${hours}:${minutes}`;
}

async function loadEvents() {
    clearElement("eventsContainer");

    try {
        const response = await fetch(`${API_BASE}/Event/schedule`);

        if (!response.ok) {
            throw new Error("Could not load events");
        }

        const events = await response.json();
        const container = document.getElementById("eventsContainer");

        events.forEach(eventItem => {
            container.innerHTML += `
                <div class="item">
                  <h3>${eventItem.title}</h3>
                  <p><b>Id:</b> ${eventItem.id}</p>
                   <p><b>Description:</b> ${eventItem.description}</p>
                   <p><b>Start:</b> ${formatDateTimeDisplay(eventItem.startDate)}</p>
                   <p><b>End:</b> ${formatDateTimeDisplay(eventItem.endDate)}</p>
                   <p><b>Location:</b> ${eventItem.location}</p>
                   <p><b>Type:</b> ${eventItem.eventType}</p>

                      <button onclick="loadEventDetails(${eventItem.id})">View Details</button>
                  </div>
                `;
        });
    }
    catch (error) {
        showError("eventsContainer", error.message);
    }
}

async function loadSessions() {
    clearElement("sessionsContainer");

    const eventId = document.getElementById("eventIdInput").value;

    if (!eventId) {
        showError("sessionsContainer", "Please enter Event Id");
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/Event/${eventId}/sessions`);

        if (!response.ok) {
            throw new Error("Could not load sessions for this event");
        }

        const sessions = await response.json();
        const container = document.getElementById("sessionsContainer");

        sessions.forEach(session => {
            container.innerHTML += `
                <div class="item">
                    <h3>${session.title}</h3>
                    <p><b>Session Id:</b> ${session.id}</p>
                    <p><b>Description:</b> ${session.description}</p>
                    <p><b>Speaker:</b> ${session.speakerName}</p>
                    <p><strong>Start:</strong> ${formatDateTimeDisplay(session.startTime)}</p>
                    <p><strong>End:</strong> ${formatDateTimeDisplay(session.endTime)}</p>
                    <p><b>Room:</b> ${session.roomName}</p>
                </div>
            `;
        });
    }
    catch (error) {
        showError("sessionsContainer", error.message);
    }
}

async function registerUserToSession() {
    const resultElement = document.getElementById("registerResult");
    resultElement.innerHTML = "";

    const sessionId = document.getElementById("registerSessionId").value;
    const userId = document.getElementById("registerUserId").value;

    if (!sessionId) {
        resultElement.innerHTML = `<span class="error">Please choose session</span>`;
        return;
    }

    if (!userId) {
        resultElement.innerHTML = `<span class="error">Please choose user</span>`;
        return;
    }

    const registration = {
        userId: parseInt(userId)
    };

    try {
        const response = await fetch(`${API_BASE}/Session/${sessionId}/register`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(registration)
        });

        const text = await response.text();

        if (!response.ok) {
            resultElement.innerHTML = `<span class="error">${text}</span>`;
            return;
        }

        resultElement.innerHTML = `<span class="success">${text}</span>`;

    }
    catch (error) {
        resultElement.innerHTML = `<span class="error">${error.message}</span>`;
    }
}

async function loadUserSchedule() {
    const container = document.getElementById("userScheduleContainer");
    container.innerHTML = "";

    const userId = document.getElementById("scheduleUserId").value;

    if (!userId) {
        container.innerHTML = `<span class="error">Please choose user</span>`;
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/User/${userId}/schedule`);

        if (!response.ok) {
            const text = await response.text();
            container.innerHTML = `<span class="error">${text}</span>`;
            return;
        }

        const schedule = await response.json();

        schedule.forEach(item => {
            container.innerHTML += `
                <div class="item">
                    <h3>${item.sessionTitle}</h3>
                    <p><strong>Event:</strong> ${item.eventTitle}</p>
                    <p><strong>Session Id:</strong> ${item.sessionId}</p>
                    <p><strong>Speaker:</strong> ${item.speakerName}</p>
                    <p><strong>Start:</strong> ${formatDateTimeDisplay(item.startTime)}</p>
                    <p><strong>End:</strong> ${formatDateTimeDisplay(item.endTime)}</p>
                    <p><strong>Room:</strong> ${item.roomName}</p>
                </div>
            `;
        });
    }
    catch (error) {
        container.innerHTML = `<span class="error">${error.message}</span>`;
    }
}

async function loadWeather() {
    const weatherContainer = document.getElementById("weatherContainer");
    weatherContainer.innerHTML = "";

    const eventId = document.getElementById("weatherEventSelect").value;

    if (!eventId) {
        weatherContainer.innerHTML = `<span class="error">Please choose event</span>`;
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/Event/${eventId}/weather`);

        if (!response.ok) {
            const text = await response.text();
            weatherContainer.innerHTML = `<span class="error">${text}</span>`;
            return;
        }

        const weather = await response.json();

        const current = weather.current;

        if (!current) {
            weatherContainer.innerHTML = `<span class="error">Weather data is not available</span>`;
            return;
        }

        const temperature = current.temperature_2m;
        const windSpeed = current.wind_speed_10m;
        const weatherCode = current.weather_code;
        const weatherDescription = getWeatherDescription(weatherCode);

        weatherContainer.innerHTML = `
            <div class="weather-box">
                <h3>Current Weather</h3>

                <p>
                    <strong>Temperature:</strong>
                    <span class="weather-value">${temperature}°C</span>
                </p>

                <p>
                    <strong>Wind Speed:</strong>
                    <span class="weather-value">${windSpeed} km/h</span>
                </p>

                <p>
                    <strong>Weather:</strong>
                    <span class="weather-value">${weatherDescription}</span>
                </p>

                <p>
                    <strong>Weather Code:</strong>
                    ${weatherCode}
                </p>
            </div>
        `;
    }
    catch (error) {
        weatherContainer.innerHTML = `<span class="error">${error.message}</span>`;
    }
}

async function loadStatistics() {
    clearElement("statisticsContainer");

    try {
        const response = await fetch(`${API_BASE}/Statistics`);

        if (!response.ok) {
            throw new Error("Could not load statistics");
        }

        const statistics = await response.json();
        const container = document.getElementById("statisticsContainer");

        let popularSessionHtml = "No registrations yet";

        if (statistics.mostPopularSession != null) {
            popularSessionHtml = `
                ${statistics.mostPopularSession.sessionTitle}
                (${statistics.mostPopularSession.registrationsCount} registrations)
            `;
        }

        let monthsHtml = "";

        statistics.eventsByMonth.forEach(month => {
            monthsHtml += `
                <li>${month.month}/${month.year}: ${month.eventsCount} events</li>
            `;
        });

        container.innerHTML = `
            <div class="item">
                <p><b>Total Events:</b> ${statistics.totalEvents}</p>
                <p><b>Total Sessions:</b> ${statistics.totalSessions}</p>
                <p><b>Total Registrations:</b> ${statistics.totalRegistrations}</p>
                <p><b>Average Registrations Per Session:</b> ${statistics.averageRegistrationsPerSession}</p>
                <p><b>Most Popular Session:</b> ${popularSessionHtml}</p>
                <p><b>Events By Month:</b></p>
                <ul>${monthsHtml}</ul>
            </div>
        `;
    }
    catch (error) {
        showError("statisticsContainer", error.message);
    }
}

async function createNewEvent() {
    const resultElement = document.getElementById("adminEventResult");
    resultElement.innerHTML = "";

    const title = document.getElementById("createEventTitle").value;
    const description = document.getElementById("createEventDescription").value;
    const startDate = document.getElementById("createEventStartDate").value;
    const endDate = document.getElementById("createEventEndDate").value;
    const location = document.getElementById("createEventLocation").value;
    const eventType = document.getElementById("createEventType").value;

    if (!title || !startDate || !endDate || !location) {
        resultElement.innerHTML = `<span class="error">Please fill title, start date, end date and location</span>`;
        return;
    }

    const newEvent = {
        id: 0,
        title: title,
        description: description,
        startDate: startDate,
        endDate: endDate,
        location: location,
        eventType: eventType
    };

    try {
        const response = await fetch(`${API_BASE}/Event`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(newEvent)
        });

        if (!response.ok) {
            const text = await response.text();
            resultElement.innerHTML = `<span class="error">${text}</span>`;
            return;
        }

        const createdEvent = await response.json();

        resultElement.innerHTML =
            `<span class="success">Event created successfully. New Event Id: ${createdEvent.id}</span>`;

        await loadEvents();
        await loadDashboardStats();
    }
    catch (error) {
        resultElement.innerHTML = `<span class="error">${error.message}</span>`;
    }
}

async function updateExistingEvent() {
    const resultElement = document.getElementById("adminEventResult");
    resultElement.innerHTML = "";

    const eventId = document.getElementById("updateEventId").value;

    if (!eventId) {
        resultElement.innerHTML = `<span class="error">Please enter Event Id</span>`;
        return;
    }

    try {
        const getResponse = await fetch(`${API_BASE}/Event/${eventId}`);

        if (!getResponse.ok) {
            resultElement.innerHTML = `<span class="error">Event not found</span>`;
            return;
        }

        const currentEvent = await getResponse.json();

        const titleInput = document.getElementById("updateEventTitle").value;
        const descriptionInput = document.getElementById("updateEventDescription").value;
        const startDateInput = document.getElementById("updateEventStartDate").value;
        const endDateInput = document.getElementById("updateEventEndDate").value;
        const locationInput = document.getElementById("updateEventLocation").value;
        const eventTypeInput = document.getElementById("updateEventType").value;

        const updatedEvent = {
            id: parseInt(eventId),
            title: titleInput || currentEvent.title,
            description: descriptionInput || currentEvent.description,
            startDate: startDateInput || currentEvent.startDate,
            endDate: endDateInput || currentEvent.endDate,
            location: locationInput || currentEvent.location,
            eventType: eventTypeInput || currentEvent.eventType
        };

        const updateResponse = await fetch(`${API_BASE}/Event/${eventId}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(updatedEvent)
        });

        const text = await updateResponse.text();

        if (!updateResponse.ok) {
            resultElement.innerHTML = `<span class="error">${text}</span>`;
            return;
        }

        resultElement.innerHTML = `<span class="success">${text}</span>`;

        await loadEvents();
        await loadDashboardStats();
    }
    catch (error) {
        resultElement.innerHTML = `<span class="error">${error.message}</span>`;
    }
}

async function deleteExistingEvent() {
    const resultElement = document.getElementById("adminEventResult");
    resultElement.innerHTML = "";

    const eventId = document.getElementById("deleteEventId").value;

    if (!eventId) {
        resultElement.innerHTML = `<span class="error">Please enter Event Id</span>`;
        return;
    }

    const confirmDelete = confirm(
        `Are you sure you want to delete Event Id ${eventId}?\n\nThis action may delete an important event.`
    );

    if (!confirmDelete) {
        resultElement.innerHTML = `<span class="error">Delete cancelled</span>`;
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/Event/${eventId}`, {
            method: "DELETE"
        });

        const text = await response.text();

        if (!response.ok) {
            resultElement.innerHTML = `<span class="error">${text}</span>`;
            return;
        }

        resultElement.innerHTML = `<span class="success">${text}</span>`;

        await loadEvents();
        await loadDashboardStats();
    }
    catch (error) {
        resultElement.innerHTML = `<span class="error">${error.message}</span>`;
    }
}

function formatDateTimeForInput(dateString) {
    if (!dateString) {
        return "";
    }

    return dateString.substring(0, 16);
}

async function loadEventForUpdate() {
    const resultElement = document.getElementById("adminEventResult");
    resultElement.innerHTML = "";

    const eventId = document.getElementById("updateEventId").value;

    if (!eventId) {
        resultElement.innerHTML = `<span class="error">Please enter Event Id</span>`;
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/Event/${eventId}`);

        if (!response.ok) {
            resultElement.innerHTML = `<span class="error">Event not found</span>`;
            return;
        }

        const eventItem = await response.json();

        document.getElementById("updateEventTitle").value = eventItem.title || "";
        document.getElementById("updateEventDescription").value = eventItem.description || "";
        document.getElementById("updateEventStartDate").value = formatDateTimeForInput(eventItem.startDate);
        document.getElementById("updateEventEndDate").value = formatDateTimeForInput(eventItem.endDate);
        document.getElementById("updateEventLocation").value = eventItem.location || "";
        document.getElementById("updateEventType").value = eventItem.eventType || "";

        resultElement.innerHTML = `<span class="success">Event loaded. Change only the fields you want and click Update Event.</span>`;
    }
    catch (error) {
        resultElement.innerHTML = `<span class="error">${error.message}</span>`;
    }
}

async function createNewSession() {
    const resultElement = document.getElementById("adminSessionResult");
    resultElement.innerHTML = "";

    const eventId = document.getElementById("createSessionEventId").value;
    const startTime = document.getElementById("createSessionStartTime").value;
    const endTime = document.getElementById("createSessionEndTime").value;

    if (!eventId) {
        resultElement.innerHTML = `<span class="error">Please enter Event Id</span>`;
        return;
    }

    if (!startTime || !endTime) {
        resultElement.innerHTML = `<span class="error">Please choose start time and end time</span>`;
        return;
    }

    const newSession = {
        id: 0,
        eventId: parseInt(eventId),
        title: document.getElementById("createSessionTitle").value,
        description: document.getElementById("createSessionDescription").value,
        speakerName: document.getElementById("createSessionSpeakerName").value,
        startTime: startTime,
        endTime: endTime,
        roomName: document.getElementById("createSessionRoomName").value
    };

    try {
        const response = await fetch(`${API_BASE}/Event/${eventId}/session`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(newSession)
        });

        const text = await response.text();

        if (!response.ok) {
            resultElement.innerHTML = `<span class="error">${text}</span>`;
            return;
        }

        resultElement.innerHTML = `<span class="success">${text}</span>`;

        document.getElementById("eventIdInput").value = eventId;
        await loadSessions();
        await loadDashboardStats();
    }
    catch (error) {
        resultElement.innerHTML = `<span class="error">${error.message}</span>`;
    }
}

async function loadSessionForUpdate() {
    const resultElement = document.getElementById("adminSessionResult");
    resultElement.innerHTML = "";

    const sessionId = document.getElementById("updateSessionSelect").value;

    if (!sessionId) {
        resultElement.innerHTML = `<span class="error">Please choose Session</span>`;
        return;
    }

    try {
        const sessionResponse = await fetch(`${API_BASE}/Session/${sessionId}`);

        if (!sessionResponse.ok) {
            resultElement.innerHTML = `<span class="error">Session not found</span>`;
            return;
        }

        const session = await sessionResponse.json();

        document.getElementById("updateSessionId").value = session.id;
        document.getElementById("updateSessionTitle").value = session.title || "";
        document.getElementById("updateSessionDescription").value = session.description || "";
        document.getElementById("updateSessionSpeakerName").value = session.speakerName || "";
        document.getElementById("updateSessionRoomName").value = session.roomName || "";

        const eventResponse = await fetch(`${API_BASE}/Event/${session.eventId}`);

        if (!eventResponse.ok) {
            resultElement.innerHTML = `<span class="error">Event not found</span>`;
            return;
        }

        const eventItem = await eventResponse.json();

        updateSessionTimeSlots = generateTimeSlots(eventItem.startDate, eventItem.endDate);

        const startOptions = updateSessionTimeSlots.slice(0, updateSessionTimeSlots.length - 1);
        const endOptions = updateSessionTimeSlots.slice(1);

        fillSelectWithOptions("updateSessionStartTime", startOptions, "Choose start time");
        fillSelectWithOptions("updateSessionEndTime", endOptions, "Choose end time");

        document.getElementById("updateSessionStartTime").value = formatDateTimeForInput(session.startTime);
        document.getElementById("updateSessionEndTime").value = formatDateTimeForInput(session.endTime);

        resultElement.innerHTML = `<span class="success">Session loaded. Change only what you want.</span>`;
    }
    catch (error) {
        resultElement.innerHTML = `<span class="error">${error.message}</span>`;
    }
}

async function updateExistingSession() {
    const resultElement = document.getElementById("adminSessionResult");
    resultElement.innerHTML = "";

    const sessionId = document.getElementById("updateSessionId").value;
    const startTime = document.getElementById("updateSessionStartTime").value;
    const endTime = document.getElementById("updateSessionEndTime").value;

    if (!sessionId) {
        resultElement.innerHTML = `<span class="error">Please load a session first</span>`;
        return;
    }

    if (!startTime || !endTime) {
        resultElement.innerHTML = `<span class="error">Please choose start time and end time</span>`;
        return;
    }

    try {
        const getResponse = await fetch(`${API_BASE}/Session/${sessionId}`);

        if (!getResponse.ok) {
            resultElement.innerHTML = `<span class="error">Session not found</span>`;
            return;
        }

        const currentSession = await getResponse.json();

        const updatedSession = {
            id: parseInt(sessionId),
            eventId: currentSession.eventId,
            title: document.getElementById("updateSessionTitle").value || currentSession.title,
            description: document.getElementById("updateSessionDescription").value || currentSession.description,
            speakerName: document.getElementById("updateSessionSpeakerName").value || currentSession.speakerName,
            startTime: startTime,
            endTime: endTime,
            roomName: document.getElementById("updateSessionRoomName").value || currentSession.roomName
        };

        const updateResponse = await fetch(`${API_BASE}/Session/${sessionId}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(updatedSession)
        });

        const text = await updateResponse.text();

        if (!updateResponse.ok) {
            resultElement.innerHTML = `<span class="error">${text}</span>`;
            return;
        }

        resultElement.innerHTML = `<span class="success">${text}</span>`;

        document.getElementById("eventIdInput").value = updatedSession.eventId;
        await loadSessions();
        await loadDashboardStats();
    }
    catch (error) {
        resultElement.innerHTML = `<span class="error">${error.message}</span>`;
    }
}

async function deleteExistingSession() {
    const resultElement = document.getElementById("adminSessionResult");
    resultElement.innerHTML = "";

    const sessionId = document.getElementById("deleteSessionId").value;

    if (!sessionId) {
        resultElement.innerHTML = `<span class="error">Please enter Session Id</span>`;
        return;
    }

    const confirmDelete = confirm(
        `Are you sure you want to delete Session Id ${sessionId}?\n\nThis action may delete an important session.`
    );

    if (!confirmDelete) {
        resultElement.innerHTML = `<span class="error">Delete cancelled</span>`;
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/Session/${sessionId}`, {
            method: "DELETE"
        });

        const text = await response.text();

        if (!response.ok) {
            resultElement.innerHTML = `<span class="error">${text}</span>`;
            return;
        }

        resultElement.innerHTML = `<span class="success">${text}</span>`;

        const eventId = document.getElementById("eventIdInput").value;

        if (eventId) {
            await loadSessions();
            await loadDashboardStats();
        }
    }
    catch (error) {
        resultElement.innerHTML = `<span class="error">${error.message}</span>`;
    }
}

function padNumber(number) {
    return number.toString().padStart(2, "0");
}

function dateToInputValue(date) {
    const year = date.getFullYear();
    const month = padNumber(date.getMonth() + 1);
    const day = padNumber(date.getDate());
    const hours = padNumber(date.getHours());
    const minutes = padNumber(date.getMinutes());

    return `${year}-${month}-${day}T${hours}:${minutes}`;
}

function generateTimeSlots(startDateString, endDateString) {
    const slots = [];
    const current = new Date(startDateString);
    const end = new Date(endDateString);

    while (current <= end) {
        slots.push(dateToInputValue(current));
        current.setMinutes(current.getMinutes() + 30);
    }

    return slots;
}

function formatSlotText(value) {
    return formatDateTimeDisplay(value);
}

function fillSelectWithOptions(selectId, values, defaultText) {
    const select = document.getElementById(selectId);
    select.innerHTML = `<option value="">${defaultText}</option>`;

    values.forEach(value => {
        select.innerHTML += `<option value="${value}">${formatSlotText(value)}</option>`;
    });
}

let createSessionTimeSlots = [];

async function loadCreateSessionTimeOptions() {
    const resultElement = document.getElementById("adminSessionResult");
    resultElement.innerHTML = "";

    const eventId = document.getElementById("createSessionEventId").value;

    if (!eventId) {
        resultElement.innerHTML = `<span class="error">Please enter Event Id</span>`;
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/Event/${eventId}`);

        if (!response.ok) {
            resultElement.innerHTML = `<span class="error">Event not found</span>`;
            return;
        }

        const eventItem = await response.json();

        createSessionTimeSlots = generateTimeSlots(eventItem.startDate, eventItem.endDate);

        const startOptions = createSessionTimeSlots.slice(0, createSessionTimeSlots.length - 1);
        const endOptions = createSessionTimeSlots.slice(1);

        fillSelectWithOptions("createSessionStartTime", startOptions, "Choose start time");
        fillSelectWithOptions("createSessionEndTime", endOptions, "Choose end time");

        resultElement.innerHTML = `<span class="success">Time options loaded for this event</span>`;
    }
    catch (error) {
        resultElement.innerHTML = `<span class="error">${error.message}</span>`;
    }
}

function updateCreateSessionEndTimes() {
    const selectedStart = document.getElementById("createSessionStartTime").value;

    if (!selectedStart) {
        return;
    }

    const endOptions = createSessionTimeSlots.filter(slot => slot > selectedStart);

    fillSelectWithOptions("createSessionEndTime", endOptions, "Choose end time");
}

let updateSessionTimeSlots = [];

async function loadSessionsForUpdateEvent() {
    const resultElement = document.getElementById("adminSessionResult");
    resultElement.innerHTML = "";

    const eventId = document.getElementById("updateSessionEventId").value;

    if (!eventId) {
        resultElement.innerHTML = `<span class="error">Please enter Event Id</span>`;
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/Event/${eventId}/sessions`);

        if (!response.ok) {
            resultElement.innerHTML = `<span class="error">No sessions found for this event</span>`;
            return;
        }

        const sessions = await response.json();
        const select = document.getElementById("updateSessionSelect");

        select.innerHTML = `<option value="">Choose session</option>`;

        sessions.forEach(session => {
            select.innerHTML += `
                <option value="${session.id}">
                    ${session.id} - ${session.title}
                </option>
            `;
        });

        resultElement.innerHTML = `<span class="success">Sessions loaded. Choose one session.</span>`;
    }
    catch (error) {
        resultElement.innerHTML = `<span class="error">${error.message}</span>`;
    }
}

function updateUpdateSessionEndTimes() {
    const selectedStart = document.getElementById("updateSessionStartTime").value;

    if (!selectedStart) {
        return;
    }

    const endOptions = updateSessionTimeSlots.filter(slot => slot > selectedStart);

    fillSelectWithOptions("updateSessionEndTime", endOptions, "Choose end time");
}

async function loadUsers() {
    try {
        const response = await fetch(`${API_BASE}/User`);

        if (!response.ok) {
            console.log("Could not load users");
            return;
        }

        const users = await response.json();

        console.log("Users loaded:", users);

        fillUsersSelect("registerUserId", users);
        fillUsersSelect("scheduleUserId", users);
        fillUsersSelect("detailsUserId", users);
    }
    catch (error) {
        console.log("Load users error:", error.message);
    }
}

function fillUsersSelect(selectId, users) {
    const select = document.getElementById(selectId);

    if (!select) {
        console.log("Select not found:", selectId);
        return;
    }

    select.innerHTML = `<option value="">Choose user</option>`;

    users.forEach(user => {
        select.innerHTML += `
            <option value="${user.id}">
                ${user.id} - ${user.fullName} (${user.email})
            </option>
        `;
    });
}

async function loadRegisterSessions() {
    const resultElement = document.getElementById("registerResult");
    resultElement.innerHTML = "";

    const eventId = document.getElementById("registerEventId").value;

    if (!eventId) {
        resultElement.innerHTML = `<span class="error">Please enter Event Id</span>`;
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/Event/${eventId}/sessions`);

        if (!response.ok) {
            const text = await response.text();
            resultElement.innerHTML = `<span class="error">${text}</span>`;
            return;
        }

        const sessions = await response.json();

        const select = document.getElementById("registerSessionId");
        select.innerHTML = `<option value="">Choose session</option>`;

        sessions.forEach(session => {
            select.innerHTML += `
                <option value="${session.id}">
                    ${session.id} - ${session.title} | ${formatDateTimeDisplay(session.startTime)} - ${formatDateTimeDisplay(session.endTime)}
                </option>
            `;
        });

        resultElement.innerHTML = `<span class="success">Sessions loaded. Choose session and user.</span>`;
    }
    catch (error) {
        resultElement.innerHTML = `<span class="error">${error.message}</span>`;
    }
}

async function loadDashboardStats() {
    const resultElement = document.getElementById("dashboardResult");
    const popularSessionElement = document.getElementById("dashboardPopularSession");

    resultElement.innerHTML = "";
    popularSessionElement.innerHTML = "";

    try {
        const response = await fetch(`${API_BASE}/Statistics`);

        if (!response.ok) {
            const text = await response.text();
            resultElement.innerHTML = `<span class="error">${text}</span>`;
            return;
        }

        const statistics = await response.json();

        document.getElementById("dashboardEvents").innerHTML = statistics.totalEvents;
        document.getElementById("dashboardSessions").innerHTML = statistics.totalSessions;
        document.getElementById("dashboardRegistrations").innerHTML = statistics.totalRegistrations;
        document.getElementById("dashboardAverage").innerHTML =
            statistics.averageRegistrationsPerSession.toFixed(2);

        if (statistics.mostPopularSession != null) {
            popularSessionElement.innerHTML = `
                <strong>Most Popular Session:</strong>
                ${statistics.mostPopularSession.sessionTitle}
                (${statistics.mostPopularSession.registrationsCount} registrations)
            `;
        }
        else {
            popularSessionElement.innerHTML = `
                <strong>Most Popular Session:</strong> No registrations yet
            `;
        }
    }
    catch (error) {
        resultElement.innerHTML = `<span class="error">${error.message}</span>`;
    }
}

async function loadWeatherEvents() {
    const weatherContainer = document.getElementById("weatherContainer");
    weatherContainer.innerHTML = "";

    try {
        const response = await fetch(`${API_BASE}/Event/schedule`);

        if (!response.ok) {
            const text = await response.text();
            weatherContainer.innerHTML = `<span class="error">${text}</span>`;
            return;
        }

        const events = await response.json();

        const select = document.getElementById("weatherEventSelect");
        select.innerHTML = `<option value="">Choose event</option>`;

        events.forEach(eventItem => {
            select.innerHTML += `
                <option value="${eventItem.id}">
                    ${eventItem.id} - ${eventItem.title} | ${eventItem.location}
                </option>
            `;
        });

        weatherContainer.innerHTML = `<span class="success">Events loaded. Choose event and click Load Weather.</span>`;
    }
    catch (error) {
        weatherContainer.innerHTML = `<span class="error">${error.message}</span>`;
    }
}

function getWeatherDescription(weatherCode) {
    if (weatherCode === 0) {
        return "Clear sky";
    }

    if (weatherCode === 1 || weatherCode === 2 || weatherCode === 3) {
        return "Partly cloudy";
    }

    if (weatherCode === 45 || weatherCode === 48) {
        return "Fog";
    }

    if (
        weatherCode === 51 || weatherCode === 53 || weatherCode === 55 ||
        weatherCode === 61 || weatherCode === 63 || weatherCode === 65
    ) {
        return "Rain";
    }

    if (weatherCode === 80 || weatherCode === 81 || weatherCode === 82) {
        return "Rain showers";
    }

    if (weatherCode === 95 || weatherCode === 96 || weatherCode === 99) {
        return "Thunderstorm";
    }

    return "Unknown weather";
}

async function loadEventDetails(eventId) {
    const detailsContainer = document.getElementById("eventDetailsContainer");
    const weatherContainer = document.getElementById("eventDetailsWeatherContainer");
    const sessionsContainer = document.getElementById("eventDetailsSessionsContainer");
    const resultElement = document.getElementById("eventDetailsResult");

    detailsContainer.innerHTML = "";
    weatherContainer.innerHTML = "";
    sessionsContainer.innerHTML = "";
    resultElement.innerHTML = "";

    try {
        const response = await fetch(`${API_BASE}/Event/${eventId}`);

        if (!response.ok) {
            const text = await response.text();
            resultElement.innerHTML = `<span class="error">${text}</span>`;
            return;
        }

        const eventDetails = await response.json();

        detailsContainer.innerHTML = `
            <div class="item">
                <h3>${eventDetails.title}</h3>
                <p><b>Event Id:</b> ${eventDetails.id}</p>
                <p><b>Description:</b> ${eventDetails.description}</p>
                <p><b>Start:</b> ${formatDateTimeDisplay(eventDetails.startDate)}</p>
                <p><b>End:</b> ${formatDateTimeDisplay(eventDetails.endDate)}</p>
                <p><b>Location:</b> ${eventDetails.location}</p>
                <p><b>Type:</b> ${eventDetails.eventType}</p>
            </div>
        `;

        await loadEventDetailsWeather(eventId);

        if (!eventDetails.sessions || eventDetails.sessions.length === 0) {
            sessionsContainer.innerHTML = `<span class="error">No sessions found for this event</span>`;
            return;
        }

        sessionsContainer.innerHTML = `<h3>Sessions</h3>`;

        eventDetails.sessions.forEach(session => {
            sessionsContainer.innerHTML += `
                <div class="item">
                    <h3>${session.title}</h3>
                    <p><b>Session Id:</b> ${session.id}</p>
                    <p><b>Description:</b> ${session.description}</p>
                    <p><b>Speaker:</b> ${session.speakerName}</p>
                    <p><b>Start:</b> ${formatDateTimeDisplay(session.startTime)}</p>
                    <p><b>End:</b> ${formatDateTimeDisplay(session.endTime)}</p>
                    <p><b>Room:</b> ${session.roomName}</p>

                    <button onclick="registerUserFromEventDetails(${session.id})">
                        Register To This Session
                    </button>
                </div>
            `;
        });

        document.getElementById("eventDetailsSection").scrollIntoView();
    }
    catch (error) {
        resultElement.innerHTML = `<span class="error">${error.message}</span>`;
    }
}

async function loadEventDetailsWeather(eventId) {
    const weatherContainer = document.getElementById("eventDetailsWeatherContainer");

    try {
        const response = await fetch(`${API_BASE}/Event/${eventId}/weather`);

        if (!response.ok) {
            weatherContainer.innerHTML = `<span class="error">Weather data is not available</span>`;
            return;
        }

        const weather = await response.json();
        const current = weather.current;

        if (!current) {
            weatherContainer.innerHTML = `<span class="error">Weather data is not available</span>`;
            return;
        }

        const temperature = current.temperature_2m;
        const windSpeed = current.wind_speed_10m;
        const weatherCode = current.weather_code;
        const weatherDescription = getWeatherDescription(weatherCode);

        weatherContainer.innerHTML = `
            <div class="weather-box">
                <h3>Weather</h3>
                <p><b>Temperature:</b> <span class="weather-value">${temperature}°C</span></p>
                <p><b>Wind Speed:</b> <span class="weather-value">${windSpeed} km/h</span></p>
                <p><b>Weather:</b> <span class="weather-value">${weatherDescription}</span></p>
            </div>
        `;
    }
    catch (error) {
        weatherContainer.innerHTML = `<span class="error">${error.message}</span>`;
    }
}

async function registerUserFromEventDetails(sessionId) {
    const resultElement = document.getElementById("eventDetailsResult");
    resultElement.innerHTML = "";

    const userId = document.getElementById("detailsUserId").value;

    if (!userId) {
        resultElement.innerHTML = `<span class="error">Please choose user before registration</span>`;
        return;
    }

    const registration = {
        userId: parseInt(userId)
    };

    try {
        const response = await fetch(`${API_BASE}/Session/${sessionId}/register`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(registration)
        });

        const text = await response.text();

        if (!response.ok) {
            resultElement.innerHTML = `<span class="error">${text}</span>`;
            return;
        }

        resultElement.innerHTML = `<span class="success">${text}</span>`;

        await loadDashboardStats();
    }
    catch (error) {
        resultElement.innerHTML = `<span class="error">${error.message}</span>`;
    }
}

document.addEventListener("DOMContentLoaded", function () {
    loadUsers();
    loadDashboardStats();
    loadWeatherEvents();
});
loadEvents();