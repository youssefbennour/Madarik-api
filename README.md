# Madarik - AI-Powered Learning Roadmap Platform

Madarik is an intelligent learning platform that leverages AI to create personalized learning roadmaps and interactive content for various subjects and skills. It helps learners navigate complex topics with structured, AI-generated learning paths.

This project was developed during SalamHack, a 7-day hackathon focused on solving real-world problems using Generative AI.

## Features

### 1. AI-Generated Learning Roadmaps
- Dynamic roadmap generation based on user requests
- Visual representation using ReactFlow
- Includes difficulty levels (Beginner, Intermediate, Advanced)
- Estimated completion time for each roadmap
- Structured with main topics and related subtopics
- Clear progression paths with topic relationships

### 2. Interactive Topics and Chapters
- AI-generated detailed chapter content
- Progressive learning paths
- Comprehensive topic coverage
- Difficulty-based content organization
- Automatic article and resource recommendations
- Integration with external documentation and tutorials

### 3. Assessment System
- Topic-level quizzes
- Chapter-specific assessments
- Automated quiz generation
- Progress tracking
- Score calculation and feedback
- Varied question types and difficulty levels

### 4. Progress Tracking
- User progress monitoring
- Last accessed topic tracking
- Completion statistics
- Learning streaks
- Module completion tracking
- Quiz attempt history

### 5. Analytics
- Progress visualization
- Completed modules tracking
- Quiz performance metrics
- Learning streak tracking
- Personalized progress insights

### 6. User Experience
- Intuitive roadmap visualization
- Seamless content progression
- Real-time progress updates
- "Continue where you left off" feature
- Anonymous access support

## Technical Stack
- Backend: .NET Core
- Database: Marten (Document DB)
- AI Integration: Groq API (with models like deepseek-r1 and gemma2-9b)
- API Documentation: OpenAPI/Swagger
- State Management: Orleans (Grain-based)

## API Features
- RESTful endpoints
- OpenAPI documentation
- Server-sent events for real-time updates
- Structured response formats
- Error handling and status codes
- Anonymous access support

## Project Structure
- Modular endpoint organization
- Clear separation of concerns
- Consistent API response patterns
- Structured data models
- Efficient data persistence
- Scalable architecture

## Getting Started

### Prerequisites
- .NET Core SDK
- Marten database
- Groq API key

### Installation
1. Clone the repository
2. Configure your environment variables
3. Run the application

### API Endpoints

#### Roadmaps
- `GET /api/roadmaps` - Get all roadmaps
- `GET /api/roadmaps/{id}` - Get roadmap by ID
- `POST /api/roadmaps` - Generate new roadmap
- `DELETE /api/roadmaps/{id}` - Delete roadmap

#### Topics
- `GET /api/roadmaps/{roadmapId}/topics/{topicId}` - Get topic details
- `GET /api/roadmaps/{roadmapId}/topics/{topicId}/quiz` - Get topic quiz
- `POST /api/roadmaps/{roadmapId}/topics/{topicId}/quiz/submit` - Submit quiz answers
- `GET /api/roadmaps/{roadmapId}/topics/{topicId}/quiz/result` - Get quiz results

#### Analytics
- `GET /api/analytics` - Get user analytics and progress

## License
This project is licensed under the MIT License - see the LICENSE file for details. 