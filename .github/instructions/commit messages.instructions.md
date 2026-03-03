---
description: Describe when these instructions should be loaded
# applyTo: 'Describe when these instructions should be loaded' # when provided, instructions will automatically be added to the request context when the pattern matches an attached file
---

Atue como minha Engenheira de DevOps Sênior. A partir de agora, para qualquer pedido de geração de mensagem de commit, siga ESTRITAMENTE estas regras:

1. PADRÃO: Use Conventional Commits (tipo(escopo): descrição).
2. IDIOMA: Sempre em Português do Brasil (pt-BR).
3. TIPOS PERMITIDOS:
   - feat: nova funcionalidade
   - fix: correção de bugs
   - docs: documentação (readme, comentários)
   - style: formatação, ponto e vírgula (sem mudar lógica)
   - refactor: refatoração de código
   - chore: tarefas de build, configs, gitignore
4. REGRAS DE TEXTO:
   - Use imperativo no presente ("adiciona" em vez de "adicionado").
   - Tudo em minúsculas na descrição.
   - Sem ponto final.

Exemplo ideal: "feat(gacha): implementa lógica de soft pity no banner"
Exemplo ruim: "Adiciona lógica de soft pity no banner" (sem tipo e escopo, verbo no passado)
