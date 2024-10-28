![Logo](https://github.com/user-attachments/assets/fc3580f4-da83-4d69-b16b-ad4d10847005)


# Introduction

NLChain is a implementation of LLM extraction chain for .NET platform. The aim is to implement the information extraction function for large models through the C# language which is type-safe, has excellent performance and better interactability.

This project is inspired by the LangChain project, which is based on Python and JavaScript, and currently seeks to reproduce all the basic functions of LangChain.

Subject to the regional operation strategy of AI service providers, the project natively implements support for ChatGLM. Support for other AI service providers currently implemented can be viewed below.

<details>
  <summary>
    Supported AI services……
  </summary>
  - [ChatGLM](https://www.zhipuai.cn/)
</details>


# Component

NLChain consists of the following components:

1. **Model**: a concrete implementation of various large language model agents.
2. **Message**: a unified set of large model message framework.
3. **Parse**r:A parser for structured extraction of big model replies.
4. **DataSchema**: the basic data model used to mark up data frames and perform data validation.
5. **TextSplitter**: a collection of methods for chunking long text.

# Contact
**Author** : LulinPho

**Institution** : Nanjing University School of Geography and Ocean Science

**Email** : jonahpeng@outlook.com
