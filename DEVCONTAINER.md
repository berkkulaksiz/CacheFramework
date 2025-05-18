Visual Studio Dev Containers Configuration
This folder contains configuration for using Visual Studio Code Dev Containers with this project.

Prerequisites
Visual Studio Code
Dev Containers extension
Docker Desktop
Getting Started
Clone the repository to your local machine
Open the project folder in Visual Studio Code
When prompted, click "Reopen in Container" or run the "Remote-Containers: Reopen in Container" command from the Command Palette (F1)
Wait for the container to build and initialize (this may take a few minutes the first time)
What's Included
The dev container configuration includes:

.NET 8 SDK
Docker-in-Docker support for running Docker commands inside the container
Git and GitHub CLI
Redis database and Redis Insight for cache management
Visual Studio Code extensions for .NET development
Pre-configured settings for development
Features
Full Development Environment: All dependencies are pre-installed and configured
Consistent Environment: Everyone on your team uses the exact same development environment
Redis Integration: Redis and Redis Insight are included for cache testing
Docker-in-Docker: Run Docker commands within the container
Git Integration: Full Git support with GitHub CLI
Performance: Mounted volumes for .NET nuget packages and dotnet installation
Tips for Working in Dev Containers
Files are mounted from your local file system, so changes persist outside the container
The Redis cache is accessible at redis:6379 within the container
Redis Insight is available at http://localhost:8001 for visual cache management
The application runs on ports 5000 (HTTP) and 5001 (HTTPS)
Auto-save is enabled, and the .NET watcher is configured to respond to file changes
Customizing
You can customize this setup by:

Modifying .devcontainer/devcontainer.json to add extensions or change settings
Editing .devcontainer/docker-compose.dev.yml to adjust container configuration
Troubleshooting
Container fails to build: Make sure Docker is running and has enough resources
Port conflicts: If you have services running on ports 5000, 5001, 6379 or 8001, modify the port mappings in .devcontainer/docker-compose.dev.yml
File changes not detected: The container uses polling for file watching; try running dotnet watch run manually
Redis connection issues: Try accessing Redis from within the container using redis-cli -h redis
