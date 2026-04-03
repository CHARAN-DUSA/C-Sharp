# ─────────────────────────────────────────────────
# STAGE 1: Build Angular App
# ─────────────────────────────────────────────────
FROM node:20 AS build

WORKDIR /app

# Copy package files first for better caching
COPY package.json package-lock.json ./

# Install all Angular dependencies
RUN npm install

# Copy all Angular source files
COPY . .

# Build Angular for production
# Uses the production configuration from angular.json
RUN npm run build -- --configuration production

# ─────────────────────────────────────────────────
# STAGE 2: Serve with Nginx
# ─────────────────────────────────────────────────
FROM nginx:alpine

# Remove default Nginx page
RUN rm -rf /usr/share/nginx/html/*

# ✅ Correct path — no /browser subfolder for your builder
COPY --from=build /app/dist/employee-frontend /usr/share/nginx/html

# Copy custom Nginx config for Angular routing
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80