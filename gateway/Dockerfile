FROM kong:latest

# Switch to root
USER root

# Copy your plugin
COPY plugins/kong-oidc /usr/local/share/lua/5.1/kong/plugins/kong-oidc

# Fix permissions
RUN chown -R kong:kong /usr/local/share/lua/5.1/kong/plugins/kong-oidc

# Switch back to Kong user
USER kong

# Set environment variable to enable plugin
ENV KONG_PLUGINS=bundled,kong-oidc
