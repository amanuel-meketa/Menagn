# Menagn Platform

Menagn Platform is a software platform that provides a comprehensive solution for managing and securing services. It consists of three main components:

- **Security Service**: A backend service built with ASP.NET to manage security and user authentication.
- **Angular UI**: A user interface built with Angular that interacts with the security API to provide an engaging user experience.
- **NGINX API Gateway**: Acts as a reverse proxy to route requests between the UI and the security services, ensuring efficient load balancing and security.

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Installation](#installation)
- [Usage](#usage)
- [Deployment](#deployment)
- [Contributing](#contributing)
- [License](#license)

## Features

- Secure user authentication and authorization.
- Microservices architecture for scalability.
- Angular-based frontend with modern UI/UX.
- Efficient routing and load balancing through NGINX API Gateway.
- Easy deployment and CI/CD pipeline using Docker and GitHub Actions.

## Architecture

The Menagn Platform is designed using a microservices architecture with three main services:

1. **Security API (ASP.NET)**: Manages authentication and authorization for the platform.
2. **Frontend (Angular)**: Provides a responsive user interface for interacting with the security services.
3. **NGINX Gateway**: Routes requests from the frontend to the appropriate backend services, enabling scalability and security.

## Installation

### Prerequisites

To get started with Menagn Platform, ensure you have the following tools installed:

- [Docker](https://www.docker.com/get-started) for containerization.
- [Node.js](https://nodejs.org/) (version 16 or later) for the Angular UI.
- [Docker Compose](https://docs.docker.com/compose/) for managing multi-container Docker applications.
- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0/) or the latest version for building and running the ASP.NET security service.

### Clone the Repository

```bash
git clone https://github.com/amanuel-meketae/menagn-platform.git
cd menagn-platform
# Security-Service
