🚗 API de Cadastro de Administradores e Veículos (JWT Bearer)
📋 Descrição

API desenvolvida em .NET 7 (Minimal API) para o cadastro e gerenciamento de administradores e veículos, com autenticação e autorização via JWT Bearer Token.

O sistema permite o registro, login e controle de acesso aos endpoints protegidos, garantindo segurança e integridade das informações.

⚙️ Tecnologias Utilizadas

.NET 7 (Minimal API)

Entity Framework Core

SQL Server

JWT Bearer Authentication

Swagger UI

🔐 Funcionalidades

Cadastro de administradores

Login e geração de token JWT

Autenticação e autorização via Bearer Token

CRUD de veículos (acesso restrito a usuários autenticados)

Documentação interativa com Swagger

🔑 Autenticação JWT

A autenticação é baseada em Bearer Token.
Após o login, o usuário recebe um token JWT que deve ser enviado no header das requisições protegidas:

Authorization: Bearer {seu_token_aqui}
