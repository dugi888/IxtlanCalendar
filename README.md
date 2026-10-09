# IxtlanCalendar

## Task tracker:
This is how many hours I worked per task:
- UI Design: implementation and idea - 3h. 
  - Hardest thing was finding package and implementing for placeholder
- Grid date population - 30-45min
  - Starting idea (algorithm) was fine, but had to learn how the grid is working in WPF
- Control fields (month, year, full date) logic - ~4h
  - Full date field was the hardest, there is a lot of validation and processing.
    - Had to change validation technique from basic one (outline textbox) as it was overridden by MahApps.Metro UI.
- Importing holidays file and displaying - 2h
  - Used simplified version of JSON with few months. Only April has non repeatable holiday (Easter, as it is not on fixed date).
  - Non-repeatable holidays are displayed only for 2026 year. It is hardcoded.
  - Name of the holiday is displayed on hover.
- Polishing, refactoring etc - 1,5h
  - Validation for custom date field


## Other:
**Packages**:
- MahApps.Metro for placeholders and UI

**Palette:** Serene Dusk from https://piktochart.com/blog/blue-dark-red-color-palette/

**Icon** - https://www.flaticon.com/free-icon/calendar_2864882