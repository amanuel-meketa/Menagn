# Stage 1: Compile and Build Angular codebase
FROM node:latest as build
WORKDIR /app

# Copy package.json and package-lock.json to install dependencies
COPY package*.json ./
RUN npm ci

# Install Angular CLI globally
RUN npm install -g @angular/cli

# Copy all source code and build the Angular app
COPY . .
RUN npm run build -- --configuration=production

# Stage 2: Serve app with NGINX
FROM nginx:latest

# Copy the built Angular app to the NGINX html directory
COPY --from=build /app/dist/platform.ui/browser /usr/share/nginx/html

# Copy custom NGINX configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expose the port NGINX will serve on
EXPOSE 8000

# Start NGINX server
CMD ["nginx", "-g", "daemon off;"]
