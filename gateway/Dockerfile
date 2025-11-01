# Use the official Kong image
FROM kong:latest

# Switch to root to copy plugin files
USER root

# Copy your OIDC plugin into Kong's Lua plugin directory
COPY plugins/kong-oidc /usr/local/share/lua/5.1/kong/plugins/kong-oidc

# Fix permissions so Kong user can access the plugin
RUN chown -R kong:kong /usr/local/share/lua/5.1/kong/plugins/kong-oidc \
    && chmod -R 755 /usr/local/share/lua/5.1/kong/plugins/kong-oidc

# Switch back to Kong user (recommended for security)
USER kong

# Enable your custom plugin alongside bundled plugins
ENV KONG_PLUGINS=bundled,kong-oidc

# Optional: set Kong to start automatically (if using this as entrypoint)
# CMD ["kong", "docker-start"]
