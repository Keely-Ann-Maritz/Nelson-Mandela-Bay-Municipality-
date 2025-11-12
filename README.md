<h1 align="center">MUNICIPALITY WEB APPLICATION</h1>

<img width="1192" height="316" alt="LogoNMB" src="https://github.com/user-attachments/assets/4790b9fa-788b-4d79-a7f2-533176de84c8" />

<br><br>

## GitHub Student Profile 

* [Keely-Ann Maritz](https://github.com/Keely-Ann/)

<br>

## Demonstration Videos

* [POE PART 1: Task 2](https://youtu.be/Qx6cqSrwLQI)
* [POE PART 2](https://youtu.be/rploC3EzzX8)
* [POE PART 3](https://youtu.be/k5vbziy10Ms)

<br>

## Implementation Report 

### Instructions for setting up the Environment Development
1. Install Visual Studio 2022. Download link: https://visualstudio.microsoft.com/downloads/
3. Open Visual Studio 2022.
4. Download the GitHub repository or clone the repository.
5. Select the run button in the top navigation bar of Visual Studio 2022.

<br>

### Instructions on how to run the application

#### User 
1. Once the application has opened, the user will be welcomed by the Home page, displaying information about the municipality, feedback from the users and buttons to navigate to different pages.
2. Select the View Report Issues button on the Home page, to view a list of issues reported by a user.
3. Select the View Feedback button on the Home page, to view the feedback and experiences of users.
4. Select the Service Request Status button on the Home page to view the status of the service the user has requested.
5. Select the Services page to view a list of basic services the municipality provides and offers the community. Users can click on the <b>Report An Issue</b> link to redirect to the Report an Issue form.
6. Select the Events and Announcements navigation bar link to view all the events and announcements.
7. Select the <b>Announcement</b> button to toggle to the announcement view.
8. Select the pagination to nagivate through the announcements.
9. Select the <b>Events</b> button to toggle back to the events view.
10. Users can search by event title, select a category from the dropdown, start and end date.
11. Select the sorting to view the events by the oldest or newest, simply by selecting from the dropdown.
12. View the recommended events based off of your searches.
13. Select the Contact navigation bar link to redirect to the contact us page, where users will be able to view gerneral information, emergency contact information and the head of departments and their emails, allowing users to click on the email button and email a specific department head.

#### Admin Portal 
1. Select the <b>Login</b> button to login as an administrator, in the navigation bar.
2. Once you have logged in, the admin will be able to add, delete and view categories.
3. Select the events navigation tab to nagivate to the events, where the admin will be able to add, delete and view the events.
4. Select the announcement navigation tab to navigate to the announcements, where the admin will be able to add, delete and view announcements.
5. Select the assign jobs navigation tab to navigate to the service request job management, admins can view assigned service requests.
6. Select the dropdown for a service request and update the status of the report issue according to the progress.
7. Once, the admin has changed the status, select the <b>Update</b> button to update the status for the user.
8. Select the <b>Assign New Job</b> button to assign a report issue to a technician.
9. Select the <b>Back</b> button to navigate back to the Job Management page, to view all assigned service requests.
10. If the admin wishes to exit the admin portal, simply click on the <b>Logout</b> button in the navigation bar.

<br>

### Data structures
The Maps JavaScript API and Places API was implemented to assist users, when reporting an issue, as the user enters a location, a dropdown of locations will display based on the user’s input, autocompleting their search and allowing the user to select the location. 

Sets were implemented to store the unique categories of the events and announcements. It prevents duplicate categories. For example, the admin wants to add two categories, they add a category with a lowercase “road related” and a category with an uppercase “Road related”. The system does not allow the admin to create the second category as the category already exists with a lowercase. 

Dictionaries were used to store the information of the events and the announcements. The event and announcement information were accessible, and the system could retrieve the information and display the information for the user to view. 

GeeksforGeeks (2025) states a queue is used to order specific data by storing and managing it. The first element is added and removed from the queue, using First-in-First-out (FIFO). Therefore, Queues were implemented to manage all the recent and upcoming events and announcements in the community. The events could be searched by using keywords, location, category, start and end date, and filtered to display by the newest or oldest events. 

According to GeeksforGeeks (2025) a priority queue is an element with a priority value.The highest priority is completed before the lowest priority. Therefore, a Priority Queue was implemented for the recommendations. Once users search for an event with the same category, the top three recommendations will display, allowing the user to view similar events. Users can click on the event card to view the event details. 

GeeksforGeeks (2025) states a Binary Search Tree is contains a node with a unique key, which allows it to be ordered a specific way, allowing for searching and adding. Therefore, a Binary Search Tree was implemented to organise and sort the service requests. It allowed the user to search for a specific service request by its ID, with ease.

GeeksforGeeks (2025) states a graph is a data structure which can be ordered by a pair of nodes, where an edge can connect the nodes to form a graph. Therefore, a Graph was used to show the connection of the service requests and to find the shortest route. The data structure assisted with the relationship between the locations and the categories to determine if the request is linked to a priority service request (urgent). 

According to GeeksforGeeks (2025) heaps is a tree-based data structure which fills from the left to the right. The heap has a min or max heap, the min has a mimimum element and the max has a maximum element. Heaps can be rearranged. Therefore, Heaps were used to determine the lowest and highest priority, to retrieve the information and to display the priority service request (urgent) for the user to view. This allows the service requests to be managed and for the admin to handle the most important requests first. The highest priority will take preference due to it displaying automatically on the top of the heap for the admin to complete. 


 <br>

## Testing 
- Usability Testing: [Survey Monkey](https://www.surveymonkey.com/r/KVPGPGN)
- Performance Testing

<br>

## Change Log 
- The recommendations were moved to the top of the events page.
- A progress bar was added to the Report Issue Form.
- When clicking on the event, a modal pops up displaying the events information.
- Changed Part 1 to use a Binary Search Tree.
- Changed the View button text colour to white to match all buttons, for consistency. 

<br>

## Part 1: User Engagement Strategy 
- Customer Feedback and Rating

<img width="1910" height="1104" alt="Feedback" src="https://github.com/user-attachments/assets/dbeae0e2-86c0-474c-8ce8-b8f50c08d7b5" />

<img width="940" height="203" alt="image" src="https://github.com/user-attachments/assets/105a3f4b-6239-423c-b9ae-ccefcd58fe34" />

<br>

## Part 1: Implementation 

### User side 
<img width="1910" height="2230" alt="image" src="https://github.com/user-attachments/assets/11ca41ff-5345-4eba-aed0-c611fe916381" />

<img width="1878" height="916" alt="image" src="https://github.com/user-attachments/assets/2d938e6d-c1d1-4f59-a48d-5c456b16a6d9" />

<img width="1872" height="907" alt="image" src="https://github.com/user-attachments/assets/7625656a-968f-48ce-87b1-d742b83e9be0" />

<br>

### Admin side
<img width="1910" height="922" alt="image" src="https://github.com/user-attachments/assets/30f737c5-5984-4e02-9c4c-6f4a01923a0f" />

<img width="1896" height="910" alt="image" src="https://github.com/user-attachments/assets/7bf5e4df-7118-432e-bd91-52b34482775f" />

<img width="1910" height="1353" alt="image" src="https://github.com/user-attachments/assets/f84a060c-7266-42ee-ba70-398222d7ee80" />

<img width="1910" height="1250" alt="image" src="https://github.com/user-attachments/assets/dbece9e7-ef80-4103-a226-b52135e6d4ca" />

<img width="1910" height="1842" alt="image" src="https://github.com/user-attachments/assets/b4f4813a-6922-4c34-8557-de9e959c39f3" />

<img width="1910" height="1056" alt="image" src="https://github.com/user-attachments/assets/7a9825b5-1968-47e2-82fe-11c83afc4106" />

<img width="1910" height="1678" alt="image" src="https://github.com/user-attachments/assets/b551cf29-45ed-4592-8f12-128920f6a4ed" />

<br>

## Part 2: Implementation

<img width="1910" height="2579" alt="image" src="https://github.com/user-attachments/assets/7ff0eb9b-1f97-4257-b45f-8e053e32953c" />

<img width="1910" height="1992" alt="image" src="https://github.com/user-attachments/assets/a016d840-e248-4ced-b614-33dc7cc11906" />

<img width="1910" height="1363" alt="Announcements" src="https://github.com/user-attachments/assets/a2f769d3-bd0f-484a-ab1b-004868cfcb11" />

<br>

## Part 3: Implementation

### User side
<img width="1867" height="898" alt="image" src="https://github.com/user-attachments/assets/c1b534ee-97f2-42f6-a7a7-415984d0ffb2" />

<img width="1910" height="1080" alt="image" src="https://github.com/user-attachments/assets/666224d7-ba5c-4c73-90ba-238e7a7f06b1" />

<img width="1910" height="922" alt="image" src="https://github.com/user-attachments/assets/6cdb6814-bbb3-4511-ada2-72cf46f1896c" />

<br>

### Admin side
<img width="1910" height="1080" alt="image" src="https://github.com/user-attachments/assets/43fa755f-0a15-41d9-b1e9-c4a67977b197" />

<img width="1883" height="913" alt="image" src="https://github.com/user-attachments/assets/2faaf7dc-60c9-41e9-9e83-cbb9400bf240" />

<img width="1910" height="922" alt="image" src="https://github.com/user-attachments/assets/957918c2-2698-4024-a0f7-311f28bfcda4" />

<img width="1902" height="912" alt="image" src="https://github.com/user-attachments/assets/638343b3-c5ff-4c2f-82f8-bb2096cc3a5e" />

<img width="1910" height="922" alt="image" src="https://github.com/user-attachments/assets/c35c0795-79f8-4f6d-9bba-b939f53deb2b" />

<img width="1910" height="1820" alt="image" src="https://github.com/user-attachments/assets/a0a81237-fe04-445d-b636-7291ea0aa927" />

<img width="1507" height="512" alt="image" src="https://github.com/user-attachments/assets/f95aa068-5877-4505-8f25-cf3e684a229a" />

<br>

### Error Handling
<img width="1910" height="922" alt="image" src="https://github.com/user-attachments/assets/02276891-e60d-42b9-977d-112163ccb1a0" />

<img width="1910" height="1191" alt="image" src="https://github.com/user-attachments/assets/c910b59a-1db6-4ad6-ad9a-b3c43ddb8651" />

<img width="1910" height="992" alt="image" src="https://github.com/user-attachments/assets/f866b979-7c0f-47cc-9d87-dc8f769a0244" />

<br>

## ARC Document
The Completion Report, Technical Recommendations and a Full Reference list is provided in the PDF document, submitted on ARC. 









