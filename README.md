# Spec-Driven Development with AI — Workshop Demo

Демо проект към workshop-а **Spec-Driven Development (SDD) with AI**.

## Какво е Spec-Driven Development?

При SDD **спецификацията е основният артефакт**, а кодът е производен от нея.
Вместо да пишем промптове „на парче“ към AI асистента, първо описваме ясно
какво искаме в спецификация — отделен `.md` файл в папка `specs/` — и оставяме
AI да имплементира по нея.

## Процесът

1. **Пишем spec** — избираме `spec-agent` в Copilot Chat и описваме идеята. Агентът първо пита за неясното, после създава нов файл в `specs/` по шаблона, със статус `Draft`.
2. **Човек одобрява** — преглеждаме spec-а и сменяме статуса на `Ready for implementation`.
3. **AI имплементира** — `/implement specs/NNN-*.md` дава задачите на `backend-agent` и `frontend-agent`.
4. **Проверяваме** — build, тестове и Acceptance criteria от спецификацията; статусът става `Implemented`.

Ако нещо не е както трябва — **първо поправяме spec-а, после кода**.

## Контекст за AI

Освен спецификациите, AI агентът чете няколко постоянни файла. Всеки отговаря на един въпрос:

| Файл | Роля | Кога се чете |
| --- | --- | --- |
| `AGENTS.md` | Правилата на проекта и на SDD процеса | Автоматично, във всеки чат |
| `specs/NNN-*.md` | **Какво** правим | От `spec-agent` и при `/implement` |
| `DESIGN.md` | **Как изглежда** — цветове, шрифтове, компоненти | От `frontend-agent`, преди всеки UI |
| `.github/agents/` | **Кой** работи — `spec-agent`, `backend-agent`, `frontend-agent` | Когато изберем агента в чата или `/implement` му даде задача |
| `.github/prompts/` | **Как** работим — `/implement` | Когато напишем `/implement` в чата |
| `.github/instructions/` | Правила за конкретна папка — UI правилата за `src/Timesheet.Web` | Автоматично, когато AI пипа файл в тази папка |

## Агенти

Всеки агент има една роля и пипа само своята част от проекта. `spec-agent` избираме директно в чата, а другите два ги пуска командата `/implement`.

| Агент | Роля | Може да променя | Пуска го | Връща |
| --- | --- | --- | --- | --- |
| [`spec-agent`](.github/agents/spec-agent.agent.md) | Анализатор: идея → спецификация. Първо задава въпроси за неясното, после пише spec-а по `specs/template.md`. Няма достъп до терминала и не пише код. | само `specs/` | Избираме го в чата | Нов spec със статус `Draft` + допусканията, които човек трябва да провери |
| [`backend-agent`](.github/agents/backend-agent.agent.md) | Backend разработчик: изпълнява `[backend]` задачите — endpoint-и, данни, DTO-та, тестове с номера на изискванията (`R1_…`). | `Timesheet.Api`, `Timesheet.Shared`, `Timesheet.Api.Tests` | `/implement` | Минаващи build и тестове + кои `R` са покрити |
| [`frontend-agent`](.github/agents/frontend-agent.agent.md) | Frontend разработчик: изпълнява `[frontend]` задачите, като следва `DESIGN.md` буквално (loading / empty / error за всеки екран). | `Timesheet.Web` | `/implement`, след backend-а | Минаващ build + стъпки за ръчна проверка в браузъра |

Как се викат:

- **`spec-agent`** — в Copilot Chat (режим Agent) избираме `spec-agent` от падащото меню с агенти и описваме идеята. Агентът задава въпросите си директно в чата, после пише spec-а.
- [**`/implement <spec>`**](.github/prompts/implement.prompt.md) — команда (prompt file), която организира няколко агента като subagents. Пуска се само за spec със статус `Ready for implementation`. Дава задачите на `backend-agent`, после на `frontend-agent`, проверява Acceptance criteria и сменя статуса на `Implemented`.

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
AGENTS.md                 # правилата за AI — зарежда се автоматично
DESIGN.md                 # дизайн системата на UI-а
.github/
  agents/                 # spec-agent, backend-agent, frontend-agent (*.agent.md)
  prompts/                # implement.prompt.md — командата /implement
  instructions/           # frontend.instructions.md — UI правила за src/Timesheet.Web
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

- [GitHub Copilot](https://code.visualstudio.com/docs/copilot/overview) във VS Code (режим Agent) — AI агент за спецификация и имплементация
