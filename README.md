# Spec-Driven Development with AI — Workshop Demo

Демо проект към workshop-а **Spec-Driven Development (SDD) with AI**.

## Какво е Spec-Driven Development?

При SDD **спецификацията е основният артефакт**, а кодът е производен от нея.
Вместо да пишем промптове „на парче“ към AI асистента, първо описваме ясно
какво искаме в спецификация — отделен `.md` файл в папка `specs/` — и оставяме
AI да имплементира по нея.

## Процесът

1. **Пишем spec** — `/spec <идея>` създава нов файл в `specs/` от шаблона, със статус `Draft`.
2. **Човек одобрява** — преглеждаме spec-а и сменяме статуса на `Ready for implementation`.
3. **AI имплементира** — `/implement specs/NNN-*.md` дава задачите на `backend-agent` и `frontend-agent`.
4. **Проверяваме** — build, тестове и Acceptance criteria от спецификацията; статусът става `Implemented`.

Ако нещо не е както трябва — **първо поправяме spec-а, после кода**.

## Контекст за AI

Освен спецификациите, AI агентът чете няколко постоянни файла. Всеки отговаря на един въпрос:

| Файл | Роля | Кога се чете |
| --- | --- | --- |
| `CLAUDE.md` | Правилата на проекта и на SDD процеса | Автоматично, във всяка сесия |
| `specs/NNN-*.md` | **Какво** правим | При `/spec` и `/implement` |
| `DESIGN.md` | **Как изглежда** — цветове, шрифтове, компоненти | От `frontend-agent`, преди всеки UI |
| `.claude/agents/` | **Кой** работи — `backend-agent`, `frontend-agent` | Когато `/implement` им даде задачи |
| `.claude/skills/` | **Как** работим — `/spec`, `/implement` | Когато извикаме командата |

## Какво съдържа една спецификация

| Секция | Въпрос, на който отговаря |
| --- | --- |
| **Goal** | Какво правим и защо? |
| **Requirements** | Какво трябва да може системата? |
| **Out of scope** | Какво съзнателно не правим? |
| **Acceptance criteria** | Как ще разберем, че е готово? |
| **Technical notes** | Технологии, ограничения, решения. |
| **Tasks** | Чеклист, който AI отмята при имплементация. |

## Защо?

- **Предвидимост** — AI работи по ясен договор, а не по догадки.
- **Проследимост** — всяка промяна в кода води началото си от изискване.
- **По-лесен review** — преглеждаме намерението, не само diff-а.
- **Споделен контекст** — спецификацията е разбираема и за бизнеса, и за екипа, и за AI.

## Структура на проекта

Демо приложението е **регистър на времето (timesheet)** — ASP.NET Core Minimal API + Blazor WebAssembly на .NET 10.

```
CLAUDE.md                 # правилата за AI — зарежда се автоматично
DESIGN.md                 # дизайн системата на UI-а
.claude/
  agents/                 # backend-agent.md, frontend-agent.md
  skills/                 # spec/SKILL.md, implement/SKILL.md
specs/                    # спецификациите — по един .md файл за всяка
  template.md             # шаблон, от който започва всяка нова спецификация
  000-solution-setup.md   # как е създаден и конфигуриран solution-ът
src/
  Timesheet.Api/          # Minimal API, EF Core InMemory
  Timesheet.Web/          # Blazor WebAssembly UI
  Timesheet.Shared/       # общи DTO-та за API и UI
tests/
  Timesheet.Api.Tests/    # xUnit тестове
Timesheet.sln
run.sh / run.ps1          # пускат API и UI заедно
```

## Стартиране

Нужен е .NET SDK 10. Един скрипт билдва и пуска двата проекта, а Ctrl+C ги спира:

```bash
./run.sh        # macOS / Linux
./run.ps1       # Windows (PowerShell 7)
```

- API: http://localhost:5100
- UI: http://localhost:5200

Тестове: `dotnet test`.

## Инструменти

- [Claude Code](https://claude.com/claude-code) — AI агент за имплементация
