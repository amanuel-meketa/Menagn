FROM kong:latest

USER root

# Copy custom OIDC plugin
COPY plugins/kong-oidc /usr/local/share/lua/5.1/kong/plugins/kong-oidc

RUN chown -R kong:kong /usr/local/share/lua/5.1/kong/plugins/kong-oidc \
    && chmod -R 755 /usr/local/share/lua/5.1/kong/plugins/kong-oidc

USER kong

ENV KONG_PLUGINS=bundled,kong-oidc
