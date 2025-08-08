# Incremental Development Plan: Semantic Search App with GUI

This plan ensures a working GUI at every step, gradually adding backend features for a seamless, always-usable application.

---

## 1. Project & GUI Scaffold
- Set up project structure.
- Create a minimal GUI (e.g., a form with a text box and search button).
- The search button does nothing yet, but the app runs.
**Files:**
	- MainWindow.xaml (WPF main window UI)
	- MainWindow.xaml.cs (WPF main window code-behind)
	- App.xaml, App.xaml.cs (WPF application entry)
	- .gitignore, README.md, plan.md

## 2. Add Static Search Results
- Wire the GUI to display hardcoded/static search results when a query is entered.
- No backend logic yet, but the UI flow is complete.
**Files:**
	- MainWindow.xaml, MainWindow.xaml.cs (update to display static results)

## 3. Integrate Data Layer
- Load and display real documents in the GUI (e.g., from a file or database).
- Search still returns all or static results.
**Files:**
	- DataLoader.cs
	- DocumentStore.cs
	- MainWindow.xaml, MainWindow.xaml.cs (update to use real data)

## 4. Embedding Service Integration
- Connect the GUI to backend logic that generates embeddings for documents (using Ollama).
- GUI can now show a “processing” indicator.
**Files:**
	- EmbeddingService.cs
	- MainWindow.xaml, MainWindow.xaml.cs (update to trigger embedding generation)

## 5. Implement Semantic Search
- Enable the GUI to send queries, get embeddings, and display semantically matched results.
- Results are now based on vector similarity.
**Files:**
	- SemanticSearch.cs
	- VectorStore.cs
	- MainWindow.xaml, MainWindow.xaml.cs (update to show semantic results)

## 6. Add Question Answering (Gemini API) with Source Viewing
- Add a “Ask a Question” feature in the GUI.
- After search, user can ask a question; the app sends context and query to Gemini API and displays the answer.
- Add a “Show Sources” button in the chat view to display the files/snippets used as context for the answer.
- Allow opening a source in a new window for detailed inspection.
**Files:**
	- GeminiQAService.cs
	- MainWindow.xaml, MainWindow.xaml.cs (update to support Q&A, show sources, open source window)
	- SourceWindow.xaml, SourceWindow.xaml.cs (new window for source display)

## 7. Polish & Expand
- Add error handling, loading spinners, and user feedback in the GUI.
- Improve UI/UX, add settings/config, and polish deployment.
**Files:**
	- MainWindow.xaml, MainWindow.xaml.cs (UI/UX improvements)
	- Logger.cs (logging)
	- appsettings.json
	- Dockerfile, deployment scripts (optional)

---

At each step, the app remains usable and demonstrates progress both in the UI and backend capabilities.
